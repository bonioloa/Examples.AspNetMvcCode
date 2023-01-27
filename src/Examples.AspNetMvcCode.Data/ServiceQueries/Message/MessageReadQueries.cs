namespace Examples.AspNetMvcCode.Data;

public class MessageReadQueries : IMessageReadQueries
{
    private readonly ContextTenant _contextTenant;
    private readonly ContextUser _contextUser;
    private readonly IDataCommandManagerTenant _dataCommandManagerTenant;
    private readonly ITenantCryptManager _cryptManagerTenant;

    public MessageReadQueries(
        ContextTenant contextTenant
        , ContextUser contextUser
        , IDataCommandManagerTenant dataCommandManagerTenant
        , ITenantCryptManager cryptManager
        )
    {
        _contextTenant = contextTenant;
        _contextUser = contextUser;
        _dataCommandManagerTenant = dataCommandManagerTenant;
        _cryptManagerTenant = cryptManager;
    }


    private record ItemUserMessageAttachment(
        long MessageId
        , string Subject
        , string Text
        , bool SenderIsBaseUser
        , string SenderUserSurname
        , string SenderUserName
        , DateTime DateAndTime

        , long AttachmentId
        , string AttachmentName
        );

    /// <summary>
    /// Nota1: per distinguere gli allegati dei messaggi
    ///     dagli allegati delle schede, gli allegati dei messaggi
    ///     hanno 'MSG' nello stato e l'id del messaggio nella fase
    /// Nota2: la query restituisce messaggi duplicati 
    ///     Da gestire nella costruzione dell'output.
    ///     Questa cosa è voluta per fare un'unica chiamata al database
    ///     poi i dati grezzi in output vengono riorganizzati con la corretta gerarchia
    /// NOTA3: ignorare join KSTO_ADEMP_DOCS
    ///     non viene usata per gli allegati messaggi
    /// NOTA4: recuperiamo solo id e nome degli allegati, sono gli unici
    ///     dati che ci servono per comporre il link di download al file stesso
    /// </summary>
    /// <returns></returns>
    public List<ItemUserMessageQr> GetItemUserChatMessageList()
    {
        //DataTable tbMessages =
        IEnumerable<ItemUserMessageAttachment> messagesAttachments =
            _dataCommandManagerTenant.Query<ItemUserMessageAttachment>(
                new CommandExecutionDb()
                {
                    //WriteCommandLog = true,
                    Parameters =
                        new HashSet<CommandParameterDb>
                        {
                            new () { Name = SqlParamsNames.CompanyGroupId, Value = _contextTenant.CompanyGroupId},//not used in query for now
                            new () { Name = SqlParamsNames.ItemId, Value = _contextUser.ItemIdCurrentlyManagedByUser},
                            new () { Name = SqlParamsNames.State, Value = MessageQrUtility.StateForMessageAttachment },
                        },
                    CommandText = $@"
                        SELECT DISTINCT
                            CONVERT(BIGINT, em.n_prog) AS {nameof(ItemUserMessageAttachment.MessageId)}
                            , em.s_oggetto AS {nameof(ItemUserMessageAttachment.Subject)}
                            , em.s_testo AS {nameof(ItemUserMessageAttachment.Text)}
                            , CONVERT(
                                BIT
                                , CASE WHEN acRel.n_iduser is null
                                        THEN 1
                                        ELSE 0
                                    END
                                ) AS {nameof(ItemUserMessageAttachment.SenderIsBaseUser)}
                            , COALESCE(acc.s_cognome,'') AS {nameof(ItemUserMessageAttachment.SenderUserSurname)}
                            , COALESCE(acc.s_nome,'') AS {nameof(ItemUserMessageAttachment.SenderUserName)}
                            , CONVERT(DATETIME2,{DbUtility.GetSqlCodeToConvertDbDateTimeToSqlDateTime("em", "d_sys", "o_sys")}) AS {nameof(ItemUserMessageAttachment.DateAndTime)}

                            , CONVERT(BIGINT,COALESCE(doc.n_docs, 0)) AS {nameof(ItemUserMessageAttachment.AttachmentId)} 
                            , COALESCE(doc.s_nome_docs, '')AS {nameof(ItemUserMessageAttachment.AttachmentName)}

                        FROM Z_WBL_AEMAIL em
                        INNER JOIN Z_WBL_KADEMP_PROCESSO
                            adPr ON em.n_cod_adempimento = adPr.n_cod_adempimento
                                AND em.n_idprocesso = adPr.n_idprocesso 
                        LEFT JOIN Z_WBL_ADOCS
                            doc ON CONVERT(VARCHAR, em.n_prog) = doc.s_idfase
                                AND doc.s_stato = {SqlParamsNames.State}
                        LEFT JOIN Z_SYS_SACCESSI 
						    acc ON em.s_from = acc.n_iduser
					    LEFT JOIN Z_WBL_KIDUSER_RELAZIONE
						    acRel ON em.s_from = acRel.n_iduser

                        WHERE em.n_cod_adempimento = {SqlParamsNames.ItemId}
                            AND em.n_prog > 0
                        ",
                });//em.n_prog > 0 condition to exclude line filled with zero to allow old website to work

        if (messagesAttachments.IsNullOrEmpty())
        {
            return new();
        }

        List<ItemUserMessageQr> messages = new();
        foreach (ItemUserMessageAttachment messageAttachment in
            messagesAttachments.DistinctBy(msgAtt => msgAtt.MessageId)
                               .OrderByDescending(msgAtt => msgAtt.DateAndTime))
        {
            messages.Add(
                new ItemUserMessageQr()
                {
                    Id = messageAttachment.MessageId,
                    Subject = _cryptManagerTenant.DecryptAndCleanString(messageAttachment.Subject),
                    Text = _cryptManagerTenant.DecryptAndCleanString(messageAttachment.Text),
                    SenderIsBaseUser = messageAttachment.SenderIsBaseUser,
                    SenderUserSurname = _cryptManagerTenant.DecryptAndCleanString(messageAttachment.SenderUserSurname),
                    SenderUserName = _cryptManagerTenant.DecryptAndCleanString(messageAttachment.SenderUserName),
                    DateAndTime = messageAttachment.DateAndTime,

                    Attachments =
                        MapAttachmentsForMessage(
                            messageAttachment.MessageId
                            , messagesAttachments
                            )
                });
        }
        return messages;
    }

    private List<FileAttachmentQr> MapAttachmentsForMessage(
        long messageId
        , IEnumerable<ItemUserMessageAttachment> messagesAttachments
        )
    {
        if (messagesAttachments.IsNullOrEmpty())
        {
            return new();
        }

        IEnumerable<ItemUserMessageAttachment> attachmentsForMessage =
            messagesAttachments.Where(msgAtt => msgAtt.MessageId == messageId);

        if (attachmentsForMessage.IsNullOrEmpty())
        {
            return new();
        }

        List<FileAttachmentQr> attachments = new();
        foreach (ItemUserMessageAttachment messageAttachment in attachmentsForMessage.OrderBy(attMsg => attMsg.AttachmentId))
        {
            if (messageAttachment.AttachmentId.Invalid())
            {
                //happens for messages without messages, see query
                continue;
            }

            attachments.Add(
                new FileAttachmentQr()
                {
                    Id = messageAttachment.AttachmentId,
                    Name = _cryptManagerTenant.DecryptAndCleanString(messageAttachment.AttachmentName),
                });
        }

        return attachments;
    }
}
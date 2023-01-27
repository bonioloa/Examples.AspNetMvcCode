localization files (*.resx) follow folder/namespace convention to work.

So one folder for each .resx file and namespace must match folder structure (leave the default namespace on file creation by visual studio)

Make also one localization service for each .resx group

Attention to HtmlTemplateLocalized. Ensure that templates never use user input or we risk opening a script injection vulnerability
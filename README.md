# WinDocIntel2025Jan
This is a Windows Form app that uses Azure's Document Intelligence (Form Recognizer) to read lines and tables from a form.  It's based on Microsoft's tutorial for a console app: https://learn.microsoft.com/en-us/azure/ai-services/document-intelligence/quickstarts/get-started-sdks-rest-api?view=doc-intel-3.0.0&preserve-view=true&pivots=programming-language-csharp  
Features I added are:
Placing rectangles of varying colors around each of the lines.
Placing rectangles around the tables.
Placing a rectangle around the line that a user selects in a combo box and displaying that line's text.

# RunnerQueueWorker

## Описание
Программа для автоматизации запуска программ с использование очереди

## Обработка веб команд
Запуск приложений:
POST https://vprofy.ru/runnerqueue/index.php/RunApplication
BODY: {"ApplicationPath":"c:\\Path\\calc.exe"}

Скачивание целиком документа GoogleSheet
POST https://vprofy.ru/runnerqueue/index.php/DownloadFile
BODY:
{"RemoteUri":"https:\/\/docs.google.com\/spreadsheets\...","FileName":"TimeSheet.xlsx"}

Проверка выполнения элеметна в очереди:
GET https://vprofy.ru/runnerqueue/index.php/NewElement

ImportFromGoogleSheet
---------------------
CredentialsFileNamePath = "credentials.json"
ApplicationName = "Google Sheets API .NET Quickstart"
SpreadsheetId = "xxx"
ImportRange = "Sheet1!A2:E";
ExcelInsertRange

## Сценарии ошибок
- не указана команда
- не найден файл?
- не удалось скачать файл GoogleSheet
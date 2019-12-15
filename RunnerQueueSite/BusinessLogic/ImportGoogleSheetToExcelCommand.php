<?php

function insertImportGoogleSheetToExcelCommand($request)
{
    // Get inserting parameters
    $insertParams = $request->getBody();

    // Validate body
    if (!isset($insertParams))
    {
        printf('Empty or invalid json in request body.');
        http_response_code(400);
        exit();
    }

    // Validate request parameters
    $spreadsheetId = $insertParams->SpreadsheetId;
    $importRange = $insertParams->ImportRange;
    $credentialsFileNamePath = $insertParams->CredentialsFileNamePath;
    $excelInsertRange = $insertParams->ExcelInsertRange;

    if (empty($spreadsheetId))
    {
        printf("Empty SpreadsheetId value in request body.");
        http_response_code(400);
        exit();
    }
    
    if (empty($importRange))
    {
        printf("Empty ImportRange value in request body.");
        http_response_code(400);
        exit();
    }
    
    if (empty($credentialsFileNamePath))
    {
        printf("Empty CredentialsFileNamePath value in request body.");
        http_response_code(400);
        exit();
    }
        
    // Add new element
    $insertParams->CommandName = "ImportFromGoogleSheetToExcel";
    $insertParams->CommandParameters = json_encode(array('SpreadsheetId' => $spreadsheetId, 'ImportRange' => $importRange, 'CredentialsFileNamePath' => $credentialsFileNamePath, 'ExcelInsertRange' => $excelInsertRange));
    
    // Insert data to runnerqueue
    $id = runnerQueueInsert($insertParams);
    if ($id<0)
    {
        printf("insertImportGoogleSheetToExcelCommand: can't enqueue command.");
        http_response_code(400);
        exit();
    }
    
    // Return result
    $json = json_encode(array('id' => $id));
    http_response_code(201); // 201: resourse created
    $site = 'https://vprofy.ru';
    header("Location: $site/" . $_SERVER['REQUEST_URI'] . "/$id");
    header("Content-Type: application/json");
    print $json;
}
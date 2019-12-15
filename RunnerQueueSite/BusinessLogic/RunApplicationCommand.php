<?php

function insertRunApplicationCommand($request)
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

    // Validate request parameter
    $appPath = $insertParams->ApplicationPath;
    if (empty($appPath))
    {
        printf("Empty ApplicationPath value in request body.");
        http_response_code(400);
        exit();
    }
    
    // Add new element
    $insertParams->CommandName = "RunApplication";
    $insertParams->CommandParameters = json_encode(array('ApplicationPath' => $appPath));
    
    // Insert data to runnerqueue
    $id = runnerQueueInsert($insertParams);
    if ($id<0)
    {
        printf("insertDownloadFileCommand: can't enqueue command.");
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
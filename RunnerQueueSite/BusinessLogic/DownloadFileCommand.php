<?php

function insertDownloadFileCommand($request)
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
    $remoteUri = $insertParams->RemoteUri;
    $fileName = $insertParams->FileName;
    $targetDirPath = $insertParams->TargetDirPath;
    
    if (empty($remoteUri))
    {
        printf("Empty RemoteUri value in request body.");
        http_response_code(400);
        exit();
    }
    
    if (empty($fileName))
    {
        printf("Empty FileName value in request body.");
        http_response_code(400);
        exit();
    }

    // Add new element
    $insertParams->CommandName = "DownloadFile";
    $insertParams->CommandParameters = json_encode(array('RemoteUri' => $remoteUri, 'FileName' => $fileName, 'TargetDirPath' => $targetDirPath));
    
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
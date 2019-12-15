<?php

function getNewQueueElement()
{
    $row = runnerQueueSelect(1);
    
    $json = json_encode($row);
    http_response_code(200);
    header('Content-Type: application/json');
    print $json;
}

function insertNewQueueElement($request)
{
    // Get inserting parameters
    $insertParams = $request->getBody();

    // Get reqource Id
    if (!isset($insertParams))
    {
        http_response_code(400);
        printf('Empty or invalid json in request body.');

        exit();
    }

    // Insert data to runnerqueue
    $id = runnerQueueInsert($insertParams);
    if ($id<0)
    {
        printf("There is a error.");
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

function updateQueueElement($request)
{
    // Get reqource Id

    // pathInfo для PUT: /newElement/1
    $resourceId = explode('/', substr($request->pathInfo, 1));
    $formatedRoute = array_shift($resourceId);
    // $resourceId[0] = 1
    // $formatedRoute = newElement

    // Get PUT parameters
    $updateParams = $request->getBody();

    if (!isset($updateParams))
    {
        http_response_code(400);
        printf('Empty or invalid json in request body.');

        exit();
    }

    // Insert data to runnerqueue
    $id = runnerQueueUpdate($resourceId[0], $updateParams);
    if ($id<0)
    {
        printf("There is a error.");
        http_response_code(400);
        exit();
    }
    
    // Return result
    http_response_code(201); // 201: resourse created
    $site = 'https://vprofy.ru';
    header("Location: $site" . $request->pathInfo);
    header("Content-Type: application/json");
}
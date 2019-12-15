<?php

include_once 'Router/Request.php';
include_once 'Router/Router.php';
include_once 'DataRepository/QueueRepository.php';
include_once 'BusinessLogic/NewQueueElement.php';
include_once 'BusinessLogic/RunApplicationCommand.php';
include_once 'BusinessLogic/DownloadFileCommand.php';
include_once 'BusinessLogic/ImportGoogleSheetToExcelCommand.php';

$router = new Router(new Request);

$router->get('/', function() {
    return <<<HTML
    <h1>Hello world</h1>
HTML;
});

// Get one unprocessed queue element
$router->get('/NewElement', function($request) {
    getNewQueueElement();
});

// Save new queue command
$router->post('/NewElement', function($request) {
    insertNewQueueElement($request);
});

// Update queue element
$router->put('/NewElement', function($request) {
    updateQueueElement($request); 
});

// Add RunApplication command
$router->post('/RunApplication', function($request) {
    insertRunApplicationCommand($request);
});

// Add DownloadFile command
$router->post('/DownloadFile', function($request) {
    insertDownloadFileCommand($request);
});

// Add ImportGoogleSheetToExcel
$router->post('/ImportFromGoogleSheetToExcel', function($request) {
    insertImportGoogleSheetToExcelCommand($request);
});


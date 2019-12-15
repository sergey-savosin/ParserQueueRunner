<?php
    function console_log($comment, $data ){
      echo "
[debug] $comment
";
      //echo json_encode( $data );
      var_dump($data);
      echo '
';
    }
    
    function runnerQueueSelect($queueStatusId)
    {
    	$db = mysqli_connect('localhost', '035496017_mysql2', 'password', 'vprofy_runnerqueue', 3306);

    	$sql = "
    	SELECT RunnerQueueId, CommandText, Parameter1, Parameter2,
    	    CONVERT_TZ(  `CreatedTime` , @@session.time_zone ,  '+00:00' ) AS `CreatedTimeUtc`,
    	    QueueStatusId
    	FROM runnerqueue WHERE QueueStatusId = 1 /* New */ LIMIT 1";
    	
    	$result = mysqli_query($db, $sql, MYSQLI_USE_RESULT);
    
        $row = mysqli_fetch_assoc($result);

    	mysqli_close($db);
        
        return $row;
    }
    
    function runnerQueueInsert($updateParams)
    {
        $db = mysqli_connect('localhost', '035496017_mysql2', 'password', 'vprofy_runnerqueue', 3306);
        if (mysqli_connect_errno()) {
            printf("Can not connect to DB: %s\n", mysqli_connect_error());
            return -1;
        }

        $commandtext = $updateParams->CommandText;
        $parameter1 = $updateParams->Parameter1;
        $parameter2 = $updateParams->Parameter2;
        if (empty($commandtext))
        {
            printf("Empty CommandText value in Request.");
            return -1;
        }
        if (empty($parameter1))
        {
            printf("Empty Parameter1 value in Request.");
            return -1;
        }
        
        $stmt = mysqli_prepare($db, "INSERT INTO runnerqueue (CommandText, Parameter1, Parameter2) VALUES (?,?,?)");
        if ($stmt)
        {
            mysqli_stmt_bind_param($stmt, "sss", $commandtext, $parameter1, $parameter2);
    
            // insert one row
            mysqli_stmt_execute($stmt);
            $newid = mysqli_insert_id($db);
            
            mysqli_stmt_close($stmt);
            mysqli_close($db);
        }
        else
        {
            printf("Can not do sql prepare");
            return -1;
        }
        
        return $newid;
    }

    function runnerQueueUpdate($updateParams, $resource)
    {
        $db = mysqli_connect('localhost', '035496017_mysql2', 'password', 'vprofy_runnerqueue', 3306);
        if (mysqli_connect_errno()) {
            printf("Can not connect to DB: %s\n", mysqli_connect_error());
            return 500;
        }
        
        $newStatusValue = $updateParams->queuestatusid;
        $newErrorMessageValue = "'".addslashes($updateParams->errormessage)."'";
        
        // datetime
        date_default_timezone_set("UTC");
        $now = date ('Y-m-d H:i:s', time());

        // Update statement.
        // ToDo: bind params
        //$stmt = $dbh->prepare("INSERT INTO REGISTRY (name, value) VALUES (:name, :value)");
        //$stmt->bindParam(':name', $name);
        //$stmt->bindParam(':value', $value);
        
        // insert one row
        //$name = 'one';
        //$value = 1;
        //$stmt->execute();
        
        $sql = "UPDATE runnerqueue SET QueueStatusId = $newStatusValue, ErrorText = $newErrorMessageValue, ModifiedTime = '$now' WHERE RunnerQueueId = $resource ";//AND QueueStatusId <> $newStatusValue";
        $result = mysqli_query($db, $sql, MYSQLI_USE_RESULT);
        $sqlerror = mysqli_error($db);
        $affectedRows = mysqli_affected_rows($db);
        
        mysqli_close($db);
        
        console_log('rows', $affectedRows);
        console_log('new status', $newStatusValue);
        console_log('error text', $newErrorMessageValue);

        if ($result)
        {
            return 0;
        }
        else
        {
        // console_log('sql', $sql);
        // console_log('$updateParams', $updateParams);
            console_log('sql error', $sqlerror);
            return 500; //error
        }
        
    }

    if ($_SERVER["REQUEST_METHOD"] == "GET")
    {
        $row = runnerQueueSelect(1); // get records with Status=1 /* New */
        
        $json = json_encode($row);
        http_response_code(200);
        header('Content-Type: application/json');
        print $json;
    
    }
    else if ($_SERVER["REQUEST_METHOD"] == "PUT")
    {
        // Get update parameters
        $body = file_get_contents('php://input');
        switch(strtolower($_SERVER["CONTENT_TYPE"]))
        {
            case "application/json":
                $updateParams = json_decode($body);
                break;
            case "text/xml":
                http_response_code(500);
                break;
            default:
                print $_SERVER["CONTENT_TYPE"];
                http_response_code(500);
                break;
        }
        
        // Get reqource Id
        
        // PATH_INFO: "/runnerqueue/1"
        $request = explode('/', substr($_SERVER['PATH_INFO'], 1));
        $resource = array_shift($request);
        
        // Do update runnerqueue
        $result = runnerQueueUpdate($updateParams, $request[0]);
        
        // Return result
        if ($result == 0)
        {
            http_response_code(201); // 200: create resourse
        }
        else
        {
            http_response_code($result); // 404 bad request
        }
        
        //console_log('body', $body);
        //console_log('updateParams', $updateParams);
        //console_log('resource', $request);

    }
    else if ($_SERVER["REQUEST_METHOD"] == "POST")
    {
        // Get inserting parameters
        $body = file_get_contents('php://input');
        
        switch(strtolower($_SERVER["CONTENT_TYPE"]))
        {
            case "application/json":
                $insertParams = json_decode($body);
                break;
            case "text/xml":
                http_response_code(500);
                break;
            default:
                print $_SERVER["CONTENT_TYPE"];
                http_response_code(500);
                break;
        }
        
        // Get reqource Id
        if (!isset($insertParams))
        {
            http_response_code(400);
            printf('Empty or invalid json in request body.');
            console_log('body', $body);

            exit();
        }
        else
        {
            // PATH_INFO: "/parserqueue"
            $request = explode('/', substr($_SERVER['PATH_INFO'], 1));

            // Do update runnerqueue
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
            $site = 'http://vprofy.ru';
            header("Location: $site/" . $_SERVER['REQUEST_URI'] . "/$id");
            header("Content-Type: application/json");
            print $json;
        }

    }
    else
    {
        http_response_code(500);
    }
?>

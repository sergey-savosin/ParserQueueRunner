<?php
    function runnerQueueSelect($queueStatusId)
    {
    	$db = mysqli_connect('localhost', '035496017_mysql2', 'password', 'vprofy_runnerqueue', 3306);
        if (mysqli_connect_errno()) {
            printf("Can not connect to DB: %s\n", mysqli_connect_error());
            return -1;
        }

    	$sql = "
    	SELECT RunnerQueueId, CommandName, CommandParameters,
    	    CreatedTime,
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

        date_default_timezone_set("UTC");
        $now = date ('Y-m-d H:i:s', time());
        
        $commandtext = $updateParams->CommandName;
        $commandParameters = $updateParams->CommandParameters;
        if (empty($commandtext))
        {
            printf("Empty CommandName value in Request.");
            return -1;
        }
        if (empty($commandParameters))
        {
            printf("Empty CommandParameters value in Request.");
            return -1;
        }
        
        $stmt = mysqli_prepare($db, "INSERT INTO runnerqueue (CommandName, CommandParameters, CreatedTime) VALUES (?,?,?)");
        if ($stmt)
        {
            mysqli_stmt_bind_param($stmt, "sss", $commandtext, $commandParameters, $now);
    
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
    
    function runnerQueueUpdate($resourceId, $updateParams)
    {
        $db = mysqli_connect('localhost', '035496017_mysql2', 'password', 'vprofy_runnerqueue', 3306);
        if (mysqli_connect_errno()) {
            printf("Can not connect to DB: %s\n", mysqli_connect_error());
            return -1;
        }
        
        $newStatusValue = $updateParams->QueueStatusId;
        $newErrorMessageValue = $updateParams->ErrorMessage;
        
        if (empty($newStatusValue))
        {
            printf("Empty QueueStatusId value in Request.");
            return -1;
        }
        
        if (empty($resourceId))
        {
            printf("Empty $resourceId in URI");
            return -1;
        }
        
        // datetime
        date_default_timezone_set("UTC");
        $now = date ('Y-m-d H:i:s', time());

        $sql = "UPDATE runnerqueue SET QueueStatusId = ?, ErrorText = ?, ModifiedTime = ? WHERE RunnerQueueId = ? ";
        $stmt = mysqli_prepare($db, $sql);
        if (!$stmt)
        {
            printf("Can not do sql prepare");
            return -1;
        }
        
        $bindResult = mysqli_stmt_bind_param($stmt, "issi", $newStatusValue, $newErrorMessageValue, $now, $resourceId);
        if (!$bindResult)
        {
            printf("Can not do sql bind");
            return -1;
        }
        
        $execResult = mysqli_stmt_execute($stmt);
        if (!$execResult)
        {
            printf("Can not do sql exec");
            $sqlError = mysqli_error($db);
            printf("error: %s", $sqlError);
            return -1;
        }
        
        $affectedRows = mysqli_affected_rows($db);
        
        mysqli_stmt_close($stmt);
        mysqli_close($db);
        
        //console_log('rows', $affectedRows);
        //console_log('new status', $newStatusValue);
        //console_log('error text', $newErrorMessageValue);

        return 1; //ok
    }

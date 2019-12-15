<!DOCTYPE html>
<html>
<head>
	<title>Запуск программы - добавление команды</title>
    <style>
        h1 {
            width: 700px; border: thick double black; margin-left: auto;
            margin-right: auto; text-align: center; font-size: x-large; padding: .5em;
            font-family: "Trebuchet MS", Arial, Helvetica, sans-serif;
            color: darkgreen; background-image: url("border.png");
            background-size: contain; margin-top: 0;
        }
        p {
            font-family: "Trebuchet MS", Arial, Helvetica, sans-serif;
        }
        .dtable {display: table; border-collapse: collapse; font-family: "Trebuchet MS", Arial, Helvetica, sans-serif;}
        .drow {display: table-row; text-align: left; padding: 8px; border: 1px solid red;}
        .drow:nth-child(even) { background-color: #f2f2f2;}
        .drow:hover {background-color: #ddd;}
        .dheader {display: table-row;
          padding-top: 12px;
          padding-bottom: 12px;
          text-align: left;
          background-color: #4CAF50;
          border: 1px solid;
          color: white;
        }
        .dcell {display: table-cell; text-align: left; padding: 8px; border: 1px solid #ddd;}
        #buttonDiv {text-align: center;}
    </style>
</head>
<body>
<?php
	require('navigation.php');
	
	// Init
	$commandName = '';
	$commandParameters = '';
	$parameterName1 = '';
	$parameterValue1 = '';
	$parameterName2 = '';
	$parameterValue2 = '';

	if (isset($_POST['submit'])){
		// process form
		$ok = true;
		$reason = '';
		$queueStatusId = 1;

		// Validation and saving
		if (!isset($_POST['commandName']) || $_POST['commandName'] === '')
		{
			$ok = false;
			$reason .= 'commandName ';
		}
		else
		{
			$commandName = $_POST['commandName'];
		}
		
		if (!isset($_POST['parameterName1']) || $_POST['parameterName1'] === '')
		{
			$ok = false;
			$reason .= 'parameterName1 ';
		}
		else
		{
			$parameterName1 = $_POST['parameterName1'];
		}

		if (!isset($_POST['parameterValue1']) || $_POST['parameterValue1'] === '')
		{
			//$ok = false;
			//$reason .= 'parameterValue1 ';
		}
		else
		{
			$parameterValue1 = $_POST['parameterValue1'];
		}

		if (!isset($_POST['parameterName2']) || $_POST['parameterName2'] === '')
		{
			//$ok = false;
			//$reason .= 'parameterName2 ';
		}
		else
		{
			$parameterName2 = $_POST['parameterName2'];
		}

		if (!isset($_POST['parameterValue2']) || $_POST['parameterValue2'] === '')
		{
			//$ok = false;
			//$reason .= 'parameterValue2 ';
		}
		else
		{
			$parameterValue2 = $_POST['parameterValue2'];
		}

		if ($ok) {
		    // make json
		    $paramArray = [
                $parameterName1 => $parameterValue1,
                $parameterName2 => $parameterValue2
            ];
            //$commandParameters = 'json_encode(array("foo" => "bar","bar" => "foo"))';
            //$commandParameters = "test";
            $commandParameters = json_encode($paramArray);
            
            // Current datetime
            date_default_timezone_set("UTC");
            $now = date ('Y-m-d H:i:s', time());
			
			// Save to db
    		$db = mysqli_connect('localhost', '035496017_mysql2', 'password', 'vprofy_runnerqueue', 3306);
            if (mysqli_connect_errno()) {
                printf("Can not connect to DB: %s\n", mysqli_connect_error());
                return -1;
            }
            $stmt = mysqli_prepare($db, "INSERT INTO runnerqueue (CommandName, CommandParameters, QueueStatusId, CreatedTime) VALUES (?, ?, ?, ?)");
            if ($stmt)
            {
                $bind_result = mysqli_stmt_bind_param($stmt, "ssis", $commandName, $commandParameters, $queueStatusId, $now);
        
                // insert one row
                $exec_result = mysqli_stmt_execute($stmt);
                $newid = mysqli_insert_id($db);
                
                mysqli_stmt_close($stmt);
                mysqli_close($db);
                
    			printf('<p>Команда добавлена в очередь. ID=%s, bind_result: %d, exec_result: %d</p>', $newid, $bind_result ? 1 : 0, $exec_result ? 1 : 0);
            }
            else
            {
                printf("Can not do sql prepare");
                return -1;
            }
            
		}
		else {
            printf('Всё ли в порядке? %s
    		<br>В каком поле ошибка: %s
    		<br>Команда: %s
    		<br>ParameterName1: %s
    		<br>ParameterValue1: %s',
    			$ok === true ? 'Да' : 'Нет',
    			$reason,
    			htmlspecialchars($commandName),
    			htmlspecialchars($parameterName1),
    			htmlspecialchars($parameterValue1),
		);


		}
	}

?>
<hr>
<b>Форма добавления команды.</b>
<br>
<p>Чтобы команду в очередь, заполните форму:</p>

<form method="post" action="">
    <div id="oblock">        
        <div class="dtable">
            <div class="drow">
                <div class="dcell">
                    Команда:
                </div>
                <div class="dcell">
                    <?php $choices = array('RunApplication','DownloadFile','UploadRangeFromExcelToGS','DownloadRangeFromGSToExcel');?>
                    <select name='commandName'>
                    <?php foreach ($choices as $choice) {
                        echo "<option>$choice</option>\n";
                    }?>
                    </select>
                </div>
            </div>
            <div class="drow">
                <div class="dcell">
                    Параметр 1:
	            </div>
	            <div class="dcell">
                    <input type="text" name="parameterName1">
	            </div>
	        </div>
            <div class="drow">
                <div class="dcell">
                    Значение 1:
	            </div>
	            <div class="dcell">
                    <input type="text" name="parameterValue1">
	            </div>
	        </div>
            <div class="drow">
                <div class="dcell">
                    Параметр 2:
	            </div>
	            <div class="dcell">
                    <input type="text" name="parameterName2">
	            </div>
	        </div>
            <div class="drow">
                <div class="dcell">
                    Значение 2:
	            </div>
	            <div class="dcell">
                    <input type="text" name="parameterValue2">
	            </div>
	        </div>
        </div>
    </div>
	<hr>
	<input type="submit" name="submit" value="Отправить команду в очередь">
</form>
</body>
</html>
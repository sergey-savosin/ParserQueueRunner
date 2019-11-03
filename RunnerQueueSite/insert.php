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
	$commandText = '';

	if (isset($_POST['submit'])){
		// process form
		$ok = true;
		$reason = '';

		// Validation and saving
		if (!isset($_POST['commandText']) || $_POST['commandText'] === '')
		{
			$ok = false;
			$reason .= 'commandText ';
		}
		else
		{
			$commandText = $_POST['commandText'];
		}
		
		if ($ok) {
			// Save to db
    		$db = mysqli_connect('localhost', '035496017_mysql2', 'password', 'vprofy_runnerqueue', 3306);
    		//$sql = 'SELECT * FROM runnerqueue ORDER BY RunnerQueueId DESC LIMIT 0, 5';
    		//$result = mysqli_query($db, $sql);
			$sql = sprintf("INSERT runnerqueue(CommandText, QueueStatusId)
				VALUES ('%s', '%s')",
				mysqli_real_escape_string($db, $commandText),
				"1"
			);
			mysqli_query($db, $sql);
			mysqli_close($db);
			echo '<p>Команда добавлена.</p>';
		}
		else {
            printf('Всё ли в порядке? %s
    		<br>В каком поле ошибка: %s
    		<br><hr>Команда: %s',
    			$ok === true ? 'Да' : 'Нет',
    			$reason,
    			htmlspecialchars($commandText),
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
	Команда для выполнения:
	            </div>
	            <div class="dcell">
    <input type="text" name="commandText" value="<?php
		echo htmlspecialchars($commandText)
	?>">     
	            </div>
	        </div>
        </div>
    </div>
	<hr>
	<input type="submit" name="submit" value="Отправить команду в очередь">
</form>
</body>
</html>
<!DOCTYPE html>
<html>
<head>
	<title>Информация о деле - добавление заявки</title>
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
	$dealNumber = '';
	$email = '';

	if (isset($_POST['submit'])){
		// process form
		$ok = true;
		$reason = '';

		// Validation and saving
		if (!isset($_POST['dealNumber']) || $_POST['dealNumber'] === '')
		{
			$ok = false;
			$reason .= 'dealNumber ';
		}
		else
		{
			$dealNumber = $_POST['dealNumber'];
		}
		
		if (!isset($_POST['email']) || $_POST['email'] === '')
		{
			$ok = false;
			$reason .= 'email ';
		}
		else
		{
			$email = $_POST['email'];
		}

		if ($ok) {
			// Save to db
    		$db = mysqli_connect('localhost', '035496017_mysql', 'password', 'vprofy_parserqueue', 3306);
    		$sql = 'SELECT * FROM parserqueue ORDER BY ParserQueueId DESC LIMIT 0, 5';
    		$result = mysqli_query($db, $sql);
			$sql = sprintf("INSERT parserqueue(ClientEmail, ClientDocNum, QueueStatusId)
				VALUES ('%s', '%s', '%s')",
				mysqli_real_escape_string($db, $email),
				mysqli_real_escape_string($db, $dealNumber),
				"1"
			);
			mysqli_query($db, $sql);
			mysqli_close($db);
			echo '<p>Заявка добавлена.</p>';
		}
		else {
            printf('Всё ли в порядке? %s
    		<br>В каком поле ошибка: %s
    		<br><hr>Номер дела: %s
    		<br>Email: %s',
    			$ok === true ? 'Да' : 'Нет',
    			$reason,
    			htmlspecialchars($dealNumber),
    			htmlspecialchars($email)
		);


		}
	}

?>
<hr>
<b>Форма добавления заявки.</b>
<br>
<p>Чтобы получить информацию по вашему делу, заполните форму:</p>

<form method="post" action="">
    <div id="oblock">        
        <div class="dtable">
            <div class="drow">
                <div class="dcell">
	Номер дела:
	            </div>
	            <div class="dcell">
    <input type="text" name="dealNumber" value="<?php
		echo htmlspecialchars($dealNumber)
	?>">     
	            </div>
	        </div>
            <div class="drow">
                <div class="dcell">
	Email, на который прислать результат:
                </div>
                <div class="dcell">
    <input type="text" name="email"value="<?php
		echo htmlspecialchars($email)
	?>">
                </div>
            </div>
        </div>
    </div>
	<hr>
	<input type="submit" name="submit" value="Отправить заявку">
</form>
</body>
</html>
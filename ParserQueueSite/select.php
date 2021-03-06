<!DOCTYPE html>
<html>
<head>
	<title>Parser Queue - Select</title>
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
        input {width: 2em; text-align: right; border: thin solid black; padding: 2px;}
        label {width: 5em;  padding-left: .5em;display: inline-block;}
        #buttonDiv {text-align: center;}
        #oblock {display: block; margin-left: auto; margin-right: auto; width: 700px;}
    </style>
</head>
<body>

<?php
	require('navigation.php');
?>
<p>Заявки в очереди (последние 10):</p>
<div class="oblock">
    <div class="dtable">
	<div class="dheader">
        <div class="dcell">Дело №</div><div class="dcell">Результат отправлять на Email</div><div class="dcell">Статус</div><div class="dcell">Создана</div>
	</div>
	<?php
		$db = mysqli_connect('localhost', '035496017_mysql', 'password', 'vprofy_parserqueue', 3306);
		$sql = 'SELECT * FROM parserqueue ORDER BY ParserQueueId DESC LIMIT 0, 10';
		$result = mysqli_query($db, $sql);

		foreach ($result as $row) {
            $statusId = htmlspecialchars($row['QueueStatusId']);
            switch ($statusId) {
                case "1":
                    $statusName = 'Новая заявка';
		            break;
                case "2":
                    $statusName = 'Обработка завершена, письмо отправлено'; //'Обрабатывается';
		            break;
                case "3":
                    $statusName = 'Обработка завершена, письмо отправлено';
		            break;
                case "4":
                    $statusName = 'Обработка завершена с ошибкой';
		            break;
                default:
                    $statusName = 'Неизвестно';
		    }
		    
			printf('<div class="drow">
			    <div class="dcell" title="%s">%s</div><div class="dcell">%s</div><div class="dcell" title="%s">%s</div><div class="dcell">%s</div>
				</div>
',
				htmlspecialchars($row['ParserQueueId']),
				htmlspecialchars($row['ClientDocNum']),
				htmlspecialchars($row['ClientEmail']),
				htmlspecialchars($row['ErrorText']),
				$statusName,
				htmlspecialchars($row['CreatedTime'])
			);
		}

		mysqli_close($db);
	?>
	</div>
</div>

</body>
</html>
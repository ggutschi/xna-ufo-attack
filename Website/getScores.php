<?php

require_once('constants.inc.php');

header ("Content-Type:text/xml");

echo "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n";

echo "<Scores>\r\n";

$scoreFile = fopen($FILEPATH, 'r');
$rawScores = @fread($scoreFile, filesize($FILEPATH));
$scores    = split($ROWDELIM, $rawScores);

array_pop($scores);		// remove last empty line

foreach ($scores as $score) {
	$scoreList = split($ESCCOLDELIM, $score);
	
	echo "	<Score>\r\n";
	echo "		<Value>" . $scoreList[0] . "</Value>\r\n";
	echo "		<Name>" . $scoreList[1] . "</Name>\r\n";
	echo "		<Date>" . $scoreList[2] . "</Date>\r\n";
	echo "	</Score>\r\n";
}

fclose($scoreFile);

echo "</Scores>";

?>

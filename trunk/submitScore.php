<?php

require_once('constants.inc.php');

if (isset($_GET['name']) && isset($_GET['score'])) {
	$name  = $_GET['name'];
	$score = $_GET['score'];
	
	$scoreFile = fopen($FILEPATH, 'r');
	$rawScores = @fread($scoreFile, filesize($FILEPATH));
	$scores    = split($ROWDELIM, $rawScores);
	$now 	   = date("d.m.Y H:i");
	
	array_pop($scores);		// remove last empty line
	
	$scoresCount = count($scores);
	
	$newScores = Array();
	
	$i = 0;
	
	for (; $i <= $scoresCount; $i++) {	// <= because if score is lower than all others
		
		$scoreRow = split($ESCCOLDELIM, $scores[$i]);
		
		if ($score < $scoreRow[0])		// add better scores
			$newScores[] = $scoreRow;
		else {							// add new score
			
			$newScores[] = Array($score, $name, $now);
			
			break;
		}
	}
	
	// add lower scores
	for (; $i < $scoresCount; $i++) {
		$scoreRow = split($ESCCOLDELIM, $scores[$i]);
	
		$newScores[] = $scoreRow;
	}
	
	$scoreFile = fopen($FILEPATH, 'w+');
	
	for ($i = 0; $i < $MAXSCORES && $i < count($newScores); $i++)
		fwrite($scoreFile, $newScores[$i][0] . $COLDELIM . $newScores[$i][1] . $COLDELIM . $newScores[$i][2] . $ROWDELIM);
	
	fclose($scoreFile);
}

?>

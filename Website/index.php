<?php
require_once('constants.inc.php');
?>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head>
<style type="text/css">
<!--
*{border:0px solid #eee;margin:0;padding:0;list-style:none}
html,body,#bg,#bg table,#bg td,#content{width:100%;height:100%;overflow:hidden; font-family: Arial, Verdana;}
#bg div{position:absolute;width:200%;height:200%;top:-50%;left:-50%; z-index:10}
#bg td{vertical-align:middle;text-align:center}
#bg img{min-height:50%;min-width:50%;margin:0 auto}

#content{
width: 500px;
z-index: 70;
position: relative;
margin: 0 auto;
margin-top: 100px;
}

#download{
width: 400px;
z-index: 70;
position: relative;
margin: 0 auto;
margin-top: 50px;
}

a:link {
font-size: 24px;
font-weight: bold;
color: #f30101;
text-transform: uppercase;
text-decoration: underline;
}
a:hover {
font-size: 24px;
font-weight: bold;
color: #000;
text-transform: uppercase;
text-decoration: underline;
}
a:visited {
font-size: 24px;
font-weight: bold;
color: #f30101;
text-transform: uppercase;
text-decoration: underline;
}


th {
  height: 20;
  font-weight: bold;
  text-align: left;
  background-color: #f30101;
}

td {
 border-top: 1px solid #999999;
 color: #000;

}
table {
 border-collapse: collapse;
}

h1 {
text-align: center;
color: #02F3F5;
padding-bottom: 20px;
text-transform: uppercase;
}

-->
</style>
</head>

<body>
<div id="content"><h1>Highscore</h1>
<table width="100%">
<th width="50%" align="left">Name</th>
<th width="15%" align="left">Score</th>
<th width="35%" align="left">Date</th>

<?php
$scoreFile = fopen($FILEPATH, 'r');
$rawScores = @fread($scoreFile, filesize($FILEPATH));
$scores    = split($ROWDELIM, $rawScores);

array_pop($scores);		// remove last empty line

foreach ($scores as $score) {
	$scoreList = split($ESCCOLDELIM, $score);
	
	echo "	<tr>\r\n";
	echo " <td>" . $scoreList[1] . "</td>\r\n";
	echo " <td>" . $scoreList[0] . "</td>\r\n";
	echo " <td>" . $scoreList[2] . "</td>\r\n";
	echo "	</tr>\r\n";
}
fclose($scoreFile);
?>
</table>
<div id="download"><p align="center"><a href="ufo-attack.exe" target="_blank">!Download UFO-Attack!</a></p></div>
</div>


<div id="bg"><div><table cellpadding="0" cellspacing="0"><tr><td><img alt="" src="start-screen.jpg" /></td></tr></table></div></div>
</body>

</html>
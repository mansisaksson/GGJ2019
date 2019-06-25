<?php
require_once dirname(__DIR__) . '/header.php';
require_once FP_ROOT_DIR . 'core/Database.php';
require_once FP_ROOT_DIR . 'core/Globals.php';
require_once FP_ROOT_DIR . 'core/Vector3.php';


// TIME CONSTANTS
$time = time();

$secondsToReset = 1200;
$remainder = $time % $secondsToReset;
$secondsTillReset = $secondsToReset - $remainder;
$cutoffTime = $time - $remainder;

// Decode JSON
$postBody = $_GET["body"];
$getParams = json_decode($postBody, true);

$Level = $getParams["Level"];
$MaxAmount = $getParams["MaxAmount"];

// Connect to DB
$conn = Database::createConnectionToDB();
if (!$conn) {
    echo "Failed to connect to database!";
    exit_script(false);
}

// ~Begin: Get Ghosts
$stmt = "SELECT * FROM ".SQL::GHOSTS_TABLE." WHERE world = ? AND time > ? ORDER BY score DESC LIMIT ?";
$stmt = $conn->prepare($stmt);
if (!$stmt) {
    echo "Ivalid SQL statment: " . $conn->error;
    exit_script(false);
}

$stmt->bind_param('sii', $Level, $cutoffTime, $MaxAmount);
if (!$stmt->execute()) {
    error_msg("Failed to get ghost info: " . $conn->error);
    exit_script(false);
}

$result = $stmt->get_result();

$players = array();
while($row = $result->fetch_assoc())
{
    $PlayerGhost = new class{};
    $PlayerGhost->PlayerId = $row["id"];
    $PlayerGhost->PlayerName = $row["name"];
    $PlayerGhost->Score = $row["score"];
    $PlayerGhost->Time = $row["time"];
    $PlayerGhost->Color = $row["color"];
    $PlayerGhost->LevelName = $row["world"];

    array_push($players, $PlayerGhost);
}
$stmt->close();
// ~End: Get Ghosts

// ~Begin: Get Ghosts Positions
foreach ($players as &$player) {
    $stmt = "SELECT * FROM ".SQL::VALUES_TABLE." WHERE ghost_id = ? ORDER BY i ASC";
    $stmt = $conn->prepare($stmt);
    if (!$stmt) {
        echo "Ivalid SQL statment: " . $conn->error;
        exit_script(false);
    }

    $stmt->bind_param('s', $player->PlayerId);
    if (!$stmt->execute()) {
        error_msg("Failed to get ghost position values: " . $conn->error);
        exit_script(false);
    }

    $result = $stmt->get_result();

    $player->Positions = array();
    while($row = $result->fetch_assoc()) {
        array_push($player->Positions, new Vector3($row["x"], $row["y"], $row["z"]));
    }
    $stmt->close();
}
// ~End: Get Ghosts Positions


// ~Begin: Get Last Winner
$stmt = "SELECT * FROM ".SQL::GHOSTS_TABLE." WHERE world = ? AND time > ? AND time < ? ORDER BY score DESC LIMIT 1";
$stmt = $conn->prepare($stmt);
if (!$stmt) {
    echo "Ivalid SQL statment: " . $conn->error;
    exit_script(false);
}

$time1 = $cutoffTime - $secondsToReset;
$time2 = $cutoffTime;
$stmt->bind_param('sii', $Level, $time1, $time2);
if (!$stmt->execute()) {
    error_msg("Failed to get ghost info: " . $conn->error);
    exit_script(false);
}

$result = $stmt->get_result();

$LastWinner = new class{};
if ($row  = $result->fetch_assoc()) {
    $LastWinner->PlayerId = $row["id"];
    $LastWinner->PlayerName = $row["name"];
    $LastWinner->Score = $row["score"];
    $LastWinner->Time = $row["time"];
    $LastWinner->Color = $row["color"];
    $LastWinner->LevelName = $row["world"];
}
$stmt->close();
// ~End: Get Last Winner


// ~Begin: Winner All Time
$stmt = "SELECT * FROM ".SQL::GHOSTS_TABLE." WHERE world = ? ORDER BY score DESC LIMIT 1";
$stmt = $conn->prepare($stmt);
if (!$stmt) {
    echo "Ivalid SQL statment: " . $conn->error;
    exit_script(false);
}

$stmt->bind_param('s', $Level);
if (!$stmt->execute()) {
    error_msg("Failed to get ghost info: " . $conn->error);
    exit_script(false);
}

$result = $stmt->get_result();

$WinnerAllTime = new class{};
if ($row  = $result->fetch_assoc()) {
    $WinnerAllTime->PlayerId = $row["id"];
    $WinnerAllTime->PlayerName = $row["name"];
    $WinnerAllTime->Score = $row["score"];
    $WinnerAllTime->Time = $row["time"];
    $WinnerAllTime->Color = $row["color"];
    $WinnerAllTime->LevelName = $row["world"];
}
$stmt->close();
// ~End: Winner All Time

$conn->close();

$finalResult = new class{};
$finalResult->LastWinner = $LastWinner;
$finalResult->WinnerAllTime = $WinnerAllTime;
$finalResult->ResettingIn = $secondsTillReset;
$finalResult->Ghosts = $players;

exit_script(true, $finalResult);
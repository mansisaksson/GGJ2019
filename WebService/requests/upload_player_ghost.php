<?php
require_once dirname(__DIR__) . '/header.php';
require_once FP_ROOT_DIR . 'core/Database.php';
require_once FP_ROOT_DIR . 'core/Globals.php';

// Decode JSON
$postBody = $_POST["body"];
$playerGhost = json_decode($postBody, true);

$conn = Database::createConnectionToDB();
if (!$conn) {
    echo "Failed to connect to database!";
    exit_script(false);
}

/** ~Begin: ADD GHOST INFO */
$stmt = $conn->prepare("INSERT INTO ".SQL::GHOSTS_TABLE." VALUES(?, ?, ?, ?, ?, ?)");
if (!$stmt) {
    echo "Invalid SQL statment: " . $conn->error;
    exit_script(false);
}

$newId = uniqid();
$currentTime = time();
$stmt->bind_param('sssdis', $newId, $playerGhost["PlayerName"], $playerGhost["LevelName"], $playerGhost["Score"], $currentTime, $playerGhost["Color"]);
if (!$stmt->execute()) {
    error_msg("Failed to add ghost info: " . $conn->error);
    exit_script(false);
}
/** ~END: ADD GHOST INFO */

/** ~BEGIN: ADD VALUE INFO */
$positions = $playerGhost["Positions"];
$index = 0;
foreach ($positions as &$pos) {
    $stmt = $conn->prepare("INSERT INTO ".SQL::VALUES_TABLE." VALUES(?, ?, ?, ?, ?)");
    if (!$stmt) {
        echo "Invalid SQL statment: " . $conn->error;
        exit_script(false);
    }

    $stmt->bind_param('siddd', $newId, $index, $pos["x"], $pos["y"], $pos["z"]);
    if (!$stmt->execute()) {
        error_msg("Failed to add position info: " . $conn->error);
        exit_script(false);
    }

    $index += 1;
}
/** ~END: ADD VALUE INFO */

$result = $stmt->get_result();
$stmt->close();
$conn->close();

exit_script(true);
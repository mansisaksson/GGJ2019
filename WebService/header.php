<?php // prepend.php - autoprepended at the top of your tree
$serverRootFolder = "/";
$requestsDir = 'php/';

define('FP_ROOT_DIR', dirname(__FILE__) . '/');
define('RP_ROOT_DIR', $serverRootFolder);

define('FP_REQUESTS_DIR', FP_ROOT_DIR . $requestsDir);
define('RP_REQUESTS_DIR', RP_ROOT_DIR . $requestsDir);

define('ERROR_ENABLED', true);
if (ERROR_ENABLED)
{
    ini_set('display_startup_errors', 1);
    ini_set('display_errors', 1);
    error_reporting(-1);
}


// Set Json headers
header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Headers: *');

ob_start(); // Turn on output buffering

?>

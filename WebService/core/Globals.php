<?php
require_once dirname(__DIR__) . '/header.php';
require_once FP_ROOT_DIR . 'core/Patterns.php';

class ServerResponse extends Singleton
{
    private $content = array();

    public function __set($key, $value)
    {
        $this->content[$key] = $value;
    }

    public function __get($value)
    {
        return $this->content[$value];
    }

    public function getJson(): string {
        return json_encode($this->content, JSON_PRETTY_PRINT);
    }
}

function exit_script(bool $success = true, $body = null)
{
    /* 
     * Clean the output to ensure that we're returning valid Json,
     * however we still want to return the errors produced by the internal php code 
     */
    if (ob_get_contents()) {
        ServerResponse::Instance()->serverOutput = ob_get_contents();
        ob_end_clean();
    }
    
    ServerResponse::Instance()->success = $success;

    if ($body !== null) {
        ServerResponse::Instance()->body = $body;
    }
    
    echo ServerResponse::Instance()->getJson();
    exit();
}

function error_msg($msg)
{
    if (ERROR_ENABLED) {
        echo "<p id=error_msg>". $msg."</p><br>";
    }
}

function warning_msg($msg)
{
    if (ERROR_ENABLED) {
        echo "<p id=warning_msg>". $msg."</p><br>";
    }
}

function log_msg($msg)
{
    //if (ERROR_ENABLED) {
        echo "<p id=log_msg>". $msg."</p><br>";
    //}
}
?>

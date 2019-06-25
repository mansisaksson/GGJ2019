<?php
require_once dirname(__DIR__) . '/header.php';

class Singleton
{
    public static function Instance()
    {
        static $instance = null;
        if($instance === null)
        {
            $instance = new static(); // Late static binding (PHP 5.3+)
        }
        return $instance;
    }

    private function __construct() {}
    //private function __clone() {}
}
?>
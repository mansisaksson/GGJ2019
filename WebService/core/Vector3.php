<?php
require_once dirname(__DIR__) . '/header.php';

class Vector3
{
    public $x = 0;
    public $y = 0;
    public $z = 0;

    function __construct($x, $y, $z)
    {
        $this->x = $x;
        $this->y = $y;
        $this->z = $z;
    }
}
<?php
require_once dirname(__DIR__) . '/header.php';
require_once FP_ROOT_DIR . 'core/MySQL.php';

class Database
{
    public static function createConnectionToDB(): ?mysqli
    {      
        $conn = new mysqli(SQL::SERVERNAME, SQL::USERNAME, SQL::PASSWORD);
        if ($conn->connect_error) {
            error_msg("Connection failed: " . $conn->connect_error);
            return null;
        }
        
        // Open Database
        $sql = "USE ".SQL::DATABASE;
        if ($conn->query($sql) !== TRUE) {
            error_msg("Could not find file table: " . $conn->error);
            return null;
        }
        
        return $conn;
    }

    static function addUser(User $user): bool
    {
        $conn = HelperFunctions::createConnectionToDB();
        if (!isset($conn)) {
            return false;
        }
        
        Database::createUserTable($conn, false); // Make sure table exists
        
        $query = "INSERT INTO ".SQL::USERS_TABLE."
                SET            
                    user_id = ?,
                    user_name = ?,
                    password = ?
                ON DUPLICATE KEY UPDATE
                    user_id = VALUES(user_id),
                    user_name = VALUES(user_name),
                    password = VALUES(password)";
        
        $stmt = $conn->prepare($query);
        if (!$stmt) {
            error_msg("Bad SQL Syntax: ". $conn->error);
            return false;
        }
        
        $stmt->bind_param('sss', $user->UserID, $user->UserName, $user->HashedUserPassword);
        
        if (!$stmt->execute()){
            error_msg("Faild to add user: ".$conn->error);
            return false;
        }
        
        $stmt->close();
        $conn->close();
        
        return true;
    }
}
?>
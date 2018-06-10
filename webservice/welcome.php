<?php
    $username = $_POST["phone"];

    //eliminate every char except 0-9
    $username = preg_replace("/[^0-9]/", '', $username);

    if(strlen($username) == 9) {
        $username = "+420".$username;
    }
    else {
        $username = "+".$username;
    }

    //if we have 9 digits left, it's probably valid.
    if (strlen($username) == 13) {
        $message = "Ahoj, vítá tě Saturnin - tvůj Pirátský sluha pro síť Signal.\nPro více informací odpověz: Saturnine?";
        shell_exec("dbus-send --system --type=method_call --print-reply --dest=\"org.asamk.Signal\" /org/asamk/Signal org.asamk.Signal.sendMessage string:\"".$message."\" array:string: string:\"".$username."\"");

        header('Location: http://saturnin.piratsky.space/?result=succ');
    }
    else {
        header('Location: http://saturnin.piratsky.space/?result=err');
    }
?>
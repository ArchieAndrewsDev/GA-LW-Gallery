<?php
if ($handle = opendir('.')) {

    while (false !== ($entry = readdir($handle))) {

        if ($entry != "." && $entry != ".." && $entry != "FindAllNavSheets.php") {

            echo "$entry ";
        }
    }

    closedir($handle);
}
?>
#!/bin/bash

mediaPath=$1
if [ -z "$mediaPath" ] ; then
	echo "No media path provided."
	exit 1
fi

MYSQL="mysql -uroot"

# drop db
$MYSQL -e "DROP DATABASE mycms;"

# restore db
$MYSQL -e "CREATE DATABASE mycms;"
$MYSQL mycms < backupdb.sql
$MYSQL -e "GRANT ALL PRIVILEGES ON mycms.* TO \"www-data\"@localhost;"

# wipe contents of media folder
rm -rf $mediaPath
mkdir -p $mediaPath

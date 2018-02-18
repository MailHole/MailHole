#!/bin/bash

$MAIL_GUID=$(uuidgen)
echo "Test mail" | mailx -S smtp=$1 -s "Test Subject" -v test@user.com

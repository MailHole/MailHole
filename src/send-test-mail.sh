#!/bin/bash

echo "Test mail" | mailx -S smtp=$1 -s "Test Subject" -v test@user.com

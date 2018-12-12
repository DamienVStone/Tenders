#!/bin/sh
while ["" == ""];
do
 newJob = $(python3 ./SeekJob.py $1 $2);

 if [$newJob == ""];
 then
  continue;
 else
   cat <<EOF | kubectl create -f -
$newJob
EOF
 fi;

 sleep 10;
done

exit 0;
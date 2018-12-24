#!/bin/bash
while ["" == ""];
do
 newJob=$(python3 ./SeekJob.py $1 $2);
 if ((${#newJob} < 1));
 then
 echo "";
 else
   cat <<EOF | kubectl create -f -
$newJob
EOF
 fi;

 sleep 10;
done

exit 0;
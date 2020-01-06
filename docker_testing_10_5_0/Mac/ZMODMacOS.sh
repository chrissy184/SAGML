#!/bin/bash
if [ -z "$2"];
then
	export hostdir=$(pwd)
else
export hostdir=$2
fi
# echo $hostdir
# echo 2nd argument $2
set -a
sed -i '' -e "/^hostdir=/d" ~/.zmod/.env 
echo hostdir="\"$hostdir\"" >> ~/.zmod/.env
source ~/.zmod/.env
# echo after sourcing .sh file $hostdir
if [[ $1 = "up" || $1 = "" || $1 = "." ]];
then
echo Starting ZMOD
#cd $hostdir
cat ${COMPOSE_CONFIG} | envsubst | docker-compose -f - up -d
# echo in $(pwd)

echo ZMOD is up now
else
        echo Stopping ZMOD
	docker stop zmk1
        cat ${COMPOSE_CONFIG} | envsubst | docker-compose -f - down
        echo ZMOD is stopped now
fi



#!/bin/bash
if [ -z "$2"];
then
	export hostdir=$(pwd)
else
export hostdir=$2
fi
set -a
sed -i '' -e "/^hostdir=/d" ~/.mlw/.env 
echo hostdir="\"$hostdir\"" >> ~/.mlw/.env
source ~/.mlw/.env
# echo after sourcing .sh file $hostdir
if [[ $1 = "up" || $1 = "" || $1 = "." ]];
then
echo Starting mlw
#cd $hostdir
cat ${COMPOSE_CONFIG} | envsubst | sudo docker-compose -f - up -d

echo mlw is up now
else
        echo Stopping mlw
	docker rm -f zmk1
        cat ${COMPOSE_CONFIG} | envsubst | sudo docker-compose -f - down
        echo mlw is stopped now
fi



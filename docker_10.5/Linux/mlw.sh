#!/bin/bash
if [ -z "$2"];
then
	export hostdir=$(pwd)
else
export hostdir=$2
fi
set -a

sed -i "/^hostdir=/d" ~/.mlw/.env 
echo hostdir=$hostdir >> ~/.mlw/.env
source ~/.mlw/.env
if [[ $1 = "up" || $1 = "" || $1 = "." ]];
then
echo Starting mlw
#cd $hostdir
cat ${COMPOSE_CONFIG} | envsubst | sudo docker-compose -f - up -d
echo MLW is up now
else
        echo Stopping mlw
	sudo docker rm -f zmk1
        cat ${COMPOSE_CONFIG} | envsubst | sudo docker-compose -f - down
        echo MLW is stopped now
fi



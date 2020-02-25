#!/bin/bash
if [ -z "$2"];
then
        export hostdir=$(pwd)
else
export hostdir=$2
fi
echo $hostdir
set -a
sed -i '' -e "/^hostdir=/d" ~/.mlw/.env
echo hostdir=$hostdir >> ~/.mlw/.env
source ~/.mlw/.env
if [[ $1 = "up" || $1 = "" || $1 = "." ]];
then
echo Starting mlw
#cd $hostdir
cat ${COMPOSE_CONFIG} | envsubst | sudo docker-compose -f - up -d
docker exec -it nginx_mlw bash -c 'rm nginx/conf.d/default.conf; service nginx restart'
docker exec -it zmm bash -c 'echo "  inside zmm" && echo alias nyoka="/publish/nyokacli/client/bin/release/netcoreapp2.1/linux-x64/publish/nyoka" >> ~/.bashrc && echo "DONE"'


echo mlw is up now
else
        echo Stopping mlw
        sudo docker stop zmk1 zmk2 zmk3
        cat ${COMPOSE_CONFIG} | envsubst | sudo docker-compose -f - down
        echo mlw is stopped now
fi

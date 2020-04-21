#!/bin/bash
if [ -z "$2"]; # get the argument from command line coming after mlw command, execute if statement if there is no arg value
then
	export hostdir=$(pwd) # current directory as value of env variable
else
export hostdir=$2 # else get the arg value from command line
fi
set -a
sed -i '' -e "/^hostdir=/d" ~/.mlw/.env # delete the value of hostdir in .env file
echo hostdir="\"$hostdir\"" >> ~/.mlw/.env # add the new value of hostdir in .env file
source ~/.mlw/.env
# echo after sourcing .sh file $hostdir
if [[ $1 = "up" || $1 = "" || $1 = "." ]]; # if the arg after mlw command is either up or a dot or nothing
then
echo Starting mlw
#cd $hostdir
cat ${COMPOSE_CONFIG} | envsubst | sudo docker-compose -f - up -d # run the docker compose file present in ~/.mlw directory from the current directory

echo mlw is up now
else
        echo Stopping mlw
	docker rm -f zmk1
        cat ${COMPOSE_CONFIG} | envsubst | sudo docker-compose -f - down # run the docker compose file present in ~/.mlw directory from the current directory
        echo mlw is stopped now
fi



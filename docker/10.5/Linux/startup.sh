mkdir ~/.mlw  # create a .mlw dir in home
cp docker-compose.yml ~/.mlw/ # copy docker compose file to the .mlw dir in home
cp mlw.sh ~/.mlw/ # copy the mlw.sh file to .mlw dir in home
cp .env ~/.mlw/ #copy .env to .mlw
sed -i "/alias mlw=/d" ~/.bashrc # delete the present alias in bashrc file
echo alias mlw="~/.mlw/mlw.sh" >> ~/.bashrc # add the new alias in bashrc file
echo alias mlw="~/.mlw/mlw.sh" >> ~/.bashrc
chmod +x ~/.mlw/mlw.sh # give executable permission
source ~/.bashrc

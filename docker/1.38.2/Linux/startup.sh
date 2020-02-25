mkdir ~/.mlw
cp docker-compose.yml ~/.mlw/
cp mlw.sh ~/.mlw/
cp .env ~/.mlw/
sed -i "/alias mlw=/d" ~/.bashrc
echo alias mlw="~/.mlw/mlw.sh" >> ~/.bashrc
echo alias mlw="~/.mlw/mlw.sh" >> ~/.bashrc
chmod +x ~/.mlw/mlw.sh
source ~/.bashrc

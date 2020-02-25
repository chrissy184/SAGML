mkdir ~/.mlw
cp docker-compose.yml ~/.mlw/
cp mlwMacOS.sh ~/.mlw/
cp .env ~/.mlw/
sed -i '' -e "/alias mlw=/d" ~/.bashrc
sed -i '' -e "/alias mlw=/d" ~/.bashrc
echo alias mlw="~/.mlw/mlw.sh" >> ~/.bashrc
echo alias mlw="~/.mlw/mlw.sh" >> ~/.bashrc
chmod +x ~/.mlw/mlwMacOS.sh
source ~/.bashrc

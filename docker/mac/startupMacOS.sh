mkdir ~/.zmod
cp docker-compose.yml ~/.zmod/
cp ZMODMacOS.sh ~/.zmod/
cp .env ~/.zmod/
sed -i '' -e "/alias ZMOD=/d" ~/.bashrc 
sed -i '' -e "/alias zmod=/d" ~/.bashrc
echo alias ZMOD="~/.zmod/ZMOD.sh" >> ~/.bashrc
echo alias zmod="~/.zmod/ZMOD.sh" >> ~/.bashrc
chmod +x ~/.zmod/ZMOD.sh
source ~/.bashrc
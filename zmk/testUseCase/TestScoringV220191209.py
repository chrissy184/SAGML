import requests
import json
import os
import sys
from zmk.settings import BASE_DIR
mainUrl = 'http://localhost:8000/api/v1'

fPath =  BASE_DIR+'/supportData2/'
print (fPath)

def loadModelToZmk(filePath):
    url=mainUrl+'/models'
    param={'filePath':filePath}
    res=requests.post(url,param)
    res=json.loads(res.text)
    return res

def scoreData(idForData, filePath):
    url=mainUrl+'/models/'+idForData+'/score'
    param={'filePath':filePath}
    res=requests.post(url,param)
    res=json.loads(res.text)
    return res


loadModelToZmk(fPath+'mpgLRModel2.pmml')
loadModelToZmk(fPath+'mpgSKModel.pmml')
loadModelToZmk(fPath+'nnMPGMOdel10.pmml')
loadModelToZmk(fPath+'wfModelLRNN.pmml')
loadModelToZmk(fPath+'imageClassifier.pmml')

listofModelsLoaded=requests.get(mainUrl+'/models').text

print ('*'*100)
print (listofModelsLoaded)
print ('*'*100)

print ('*'*100)
r1=scoreData('mpgLRModel2.pmml', fPath+'mpg_data_exampleTest.csv')
print (r1['result'])

print ('*'*100)
r2=scoreData('mpgSKModel.pmml', fPath+'mpg_data_exampleTest.csv')
print(r2['result'])

print ('*'*100)
r3=scoreData('nnMPGMOdel10.pmml', fPath+'mpgFeatureData.csv')
print(r3['result'])

print ('*'*100)
r4=scoreData('wfModelLRNN.pmml', fPath+'mpg_data_exampleTest.csv')
print (r4['result'])

print ('*'*100)
r5=scoreData('imageClassifier.pmml', fPath+'cat_0000002.jpg')
print(r5)
print (r5['result'])

print ('*'*100)
print ('*'*100)
print ('All Successfull')
print ('*'*100)
print ('*'*100)
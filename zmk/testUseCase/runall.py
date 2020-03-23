# runall.py
import os
from zmk.settings import BASE_DIR
try:
	os.makedirs(BASE_DIR+'/testUseCase/expandable/')
except:
	pass
from subprocess import call

# print ('TestArchitectures.py >>>>>>>>>>>>>> ')
# call(["python", "./testUseCase/TestArchitectures.py"])

print (' TestAutoML.py >>>>>>>>>>>>>> ')
call(["python", BASE_DIR+"/testUseCase/TestAutoML.py"])


print (' TestDNN.py>>>>>>>>>>>>>> ')
call(["python", "BASE_DIR+/testUseCase/TestDNN.py"])


print ('TestEditorForCNN.py >>>>>>>>>>>>>> ')
call(["python", BASE_DIR+"/testUseCase/TestEditorForCNN.py"])

print ('TestScoring.py >>>>>>>>>>>>>> ')
call(["python", BASE_DIR+"/testUseCase/TestScoring.py"])

print ('TrainDNN.py >>>>>>>>>>>>>> ')
call(["python", BASE_DIR+"/testUseCase/TrainDNN.py"])


fileList = os.listdir(BASE_DIR+'/testUseCase/expandable/')
print (fileList)
for f in fileList:
	print (f)
	os.remove(BASE_DIR+'/testUseCase/expandable/'+f)

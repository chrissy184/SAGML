import time
import sys
import ast,json
from zmk.settings import BASE_DIR
def testCounter(upto):
	# upto = ast.literal_eval(upto)
	for i in range(upto):
		print(i)
		time.sleep(2)

def justTest(name,age):
	print("name ",name," age ",age+1)
        


def main(*argv):
	sys.argv = argv[:]
# # def main(*argv):
# 	# sys.argv = argv[:]
	args = sys.argv[1:]
# 	print(args)
	# testCounter(*args)
	justTest(*args)

	from trainModel import kerasUtilities
	kerasUtilities=kerasUtilities.KerasUtilities()
	kerasUtilities.updateStatusOfTraining(BASE_DIR+'/logs/UXEWJBAGBGDX/status.txt','Code Execution Completed')
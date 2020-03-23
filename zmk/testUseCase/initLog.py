import logging, sys, os
from zmk.settings import BASE_DIR

def initiateLogging():
    root = logging.getLogger()
    root.setLevel(logging.DEBUG)

    # handler = logging.StreamHandler(sys.stdout)
    # handler.setLevel(logging.DEBUG)
    # formatter = logging.Formatter('%(levelname)s - %(message)s')
    # handler.setFormatter(formatter)
    # root.addHandler(handler)

    handler2 = logging.FileHandler(BASE_DIR+'/testUseCase/ZmkUnitTestLog.log')
    handler2.setLevel(logging.DEBUG)
    formatter = logging.Formatter('%(asctime)s - %(levelname)s - %(message)s')
    handler2.setFormatter(formatter)
    root.addHandler(handler2)

def addFeatureDataStruc(data):
    data['LenCarName']=data['car name'].apply(lambda x: len(x.split()))
    newFeat=[ 'cylinders', 'displacement', 'horsepower', 'weight','acceleration','lrScore','LenCarName']
    return data[newFeat],data[['mpg']]
    
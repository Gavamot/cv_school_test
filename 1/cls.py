"""
Бинарный классификатор.
Использование:
cls.py data_dir

Аргументы:
data_dir
	Каталог с датасетами. Должен содержать каталоги train, validation (опционально), test (опционально).
	В каждом из них должны быть каталоги pos и neg, содержащие изображения в формате png, jpg или bmp.
"""

#coding=utf-8

import sys
import os
import cv2

cls_threshold = 0.5 #порог принятия решения. Если отклик классификатора > cls_threshold, то объект относится к классу 'pos'

data_dir = sys.argv[1]

train_data = []
validation_data = []
test_data = []

def classify(img):
	"""
	Получает изображение и выполняет классификацию.
	Возвращаемое значение должно лежать в интервале [0,1]. При этом если значение меньше или равно cls_threshold,
	то изображение интерпретируется как относящееся к классу 'neg', иначе -- 'pos'.
	
	Parameters:
	img : ndarray
	
	Returns:
	a : float
	"""
		
	return 1
	
def get_accuracy(results):
	"""
	вычисляет метрику качества 'accuracy' -- долю правильных ответов в выборке
	"""
	tp = 0; #true positive
	tn = 0; #true negative
	fp = 0; #false positive
	fn = 0; #false negative
	n = 0; #общее число образцов
	
	for r in results:
		(gt, a) = r
		if a > cls_threshold:
			a_cls = 1
		else:
			a_cls = 0
		
		if  gt == 1 and a_cls == 1:
			tp = tp+1
		if  gt == 1 and a_cls == 0:
			fn = fn+1
		if  gt == 0 and a_cls == 0:
			tn = tn+1
		if  gt == 0 and a_cls == 1:
			fp = fp+1
		n = n+1
	
	if n > 0:
		acc = (tp+tn)/n
	else:
		acc = 0
	return acc

def load_data(dir, gt):
	"""
	Загружает данные одного класса из каталога dir. Присваивает истинные значения gt объектам данного класса.
	
	Parameters:
	dir : string
	gt : int
		0 - для объектов класса 'neg'
		1 - для объектов класса 'pos'
	
	Returns:
	data : list of (fn : string, img : ndarray, gt : int)
	"""
	data = []
	fns = os.listdir(dir)
	for fn in fns:
		if fn.endswith(".png") or fn.endswith(".bmp") or fn.endswith(".jpg"):
			img = cv2.imread(os.path.join(dir,fn))
			data.append((fn, img, gt))
	return data

def load_dataset(dir):
	"""
	Загружает датасет из каталога dir. Датасет содержит объекты положительного и отрицательного классов.
	
	Parameters:
	dir : string
	
	Returns:
	data : list of (fn : string, img : ndarray, gt : int)
	"""
	data = []
	
	try:
		dir_pos = os.path.join(dir,'pos')
		dir_neg = os.path.join(dir,'neg')
	
		data.extend(load_data(dir_pos,1))
		data.extend(load_data(dir_neg,0))
	except:
		print("Данные в "+dir+" не найдены")
	
	return data

def load_train_dataset(dir):
	return load_dataset(os.path.join(dir, 'train'))
	
def load_validation_dataset(dir):
	return load_dataset(os.path.join(dir, 'validation'))
	
def load_test_dataset(dir):
	return load_dataset(os.path.join(dir, 'test'))
	
def process(data, dataset_name):
	if len(data) == 0:
		return

	print(dataset_name+" results:")
	
	results = []
	for d in data:
		(fn, img, gt) = d
		a = classify(img)
		results.append((gt,a))
		print("  "+fn.ljust(20), gt, a)
		
	acc = get_accuracy(results)
	print("accuracy={}\n".format(acc))

train_data = load_train_dataset(data_dir)
validation_data = load_validation_dataset(data_dir)
test_data = load_test_dataset(data_dir)

process(train_data,"train")
process(validation_data,"validation")
process(test_data,"test")
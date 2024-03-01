import matplotlib
import PyQt5
import sys
matplotlib.use('QtAgg')
from matplotlib.backends.backend_qtagg import FigureCanvasQTAgg as FigureCanvas
from matplotlib.figure import Figure
from PyQt5 import QtWidgets, QtCore, QtGui, Qt
from PyQt5.QtCore import Qt
from blackJackQ import CasinoPlayer, QLearnerPlayer, randomPlayer, Blackjack
from copy import deepcopy
from tqdm import tqdm
import matplotlib.pyplot as plt
import numpy as np

MAXEPOCHS = 150000

class MainWindow(QtWidgets.QMainWindow):

	def __init__(self, parent=None):

		QtWidgets.QMainWindow.__init__(self, parent)

		self.played = False
		self.refresh = 2

		self.cw = QtWidgets.QWidget(self)
		self.setCentralWidget(self.cw)
		self.resize(200, 800)

		self.frame = QtWidgets.QFrame()
		self.frame.setFrameShadow(QtWidgets.QFrame.Sunken)
		self.frame.setFrameShape(QtWidgets.QFrame.Panel)
		self.grid = QtWidgets.QGridLayout()

		fig = np.zeros((22,13))
		placeholder = plt.imshow(fig, vmin=-1, vmax=1).figure
		self.vis = FigureCanvas(placeholder)

		self.runButton = QtWidgets.QPushButton("Run")
		self.iterationLabel = QtWidgets.QLabel("Iteration")
		self.gammaLabel = QtWidgets.QLabel("Gamma")
		self.alphaLabel = QtWidgets.QLabel("Alpha")
		self.epsilonLabel = QtWidgets.QLabel("Epsilon")
		self.epsilonDecayLabel = QtWidgets.QLabel("Epsilon Decay")
		self.minEpsilonLabel = QtWidgets.QLabel("Minimum Epsilon")
		self.opponentLabel = QtWidgets.QLabel("Opponent")
		self.epochsLabel = QtWidgets.QLabel("Epochs")
		self.iterationReader = QtWidgets.QLabel("")
		self.gammaSlider = QtWidgets.QSlider(Qt.Horizontal)
		self.alphaSlider = QtWidgets.QSlider(Qt.Horizontal)
		self.epsilonSlider = QtWidgets.QSlider(Qt.Horizontal)
		self.epsilonDecaySlider = QtWidgets.QSlider(Qt.Horizontal)
		self.minEpsilonSlider = QtWidgets.QSlider(Qt.Horizontal)
		self.epochsSlider = QtWidgets.QSlider(Qt.Horizontal)
		self.opponentCombobox = QtWidgets.QComboBox()
		self.opponentCombobox.addItem('Casino')
		self.opponentCombobox.addItem('Random')
		# View policy or view Q-Table
		self.Qbutton = QtWidgets.QRadioButton("Q-Table")
		self.policyButton = QtWidgets.QRadioButton("Policy")
		self.visButtons = QtWidgets.QButtonGroup()
		self.visButtons.addButton(self.Qbutton)
		self.visButtons.addButton(self.policyButton)
		# View soft policy or hard policy
		self.softButton = QtWidgets.QRadioButton("Soft total")
		self.hardButton = QtWidgets.QRadioButton("Hard total")
		self.softHardButton = QtWidgets.QButtonGroup()
		self.softHardButton.addButton(self.softButton)
		self.softHardButton.addButton(self.hardButton)
		self.iterationSlider = QtWidgets.QSlider(Qt.Horizontal)

		self.grid.addWidget(self.gammaLabel, 0, 0)
		self.grid.addWidget(self.gammaSlider, 1, 0, 1, 2)
		self.grid.addWidget(self.alphaLabel, 2, 0)
		self.grid.addWidget(self.alphaSlider, 3, 0, 1, 2)
		self.grid.addWidget(self.epsilonLabel, 4, 0)
		self.grid.addWidget(self.epsilonSlider, 5, 0, 1, 2)
		self.grid.addWidget(self.epsilonDecayLabel, 6, 0)
		self.grid.addWidget(self.epsilonDecaySlider, 7, 0, 1, 2)
		self.grid.addWidget(self.minEpsilonLabel, 8, 0)
		self.grid.addWidget(self.minEpsilonSlider, 9, 0, 1, 2)
		self.grid.addWidget(self.epochsLabel, 10, 0)
		self.grid.addWidget(self.epochsSlider, 11, 0, 1, 2)
		self.grid.addWidget(self.opponentLabel, 12, 0)
		self.grid.addWidget(self.opponentCombobox, 13, 0, 1, 2)
		self.grid.addWidget(self.runButton, 14, 0, 1, 2)
		self.grid.addWidget(self.vis, 0, 2, 10, 2)
		self.grid.addWidget(self.Qbutton, 11, 2)
		self.grid.addWidget(self.policyButton, 11, 3)
		self.grid.addWidget(self.softButton, 12, 2)
		self.grid.addWidget(self.hardButton, 12, 3)
		self.grid.addWidget(self.iterationLabel, 13, 2)
		self.grid.addWidget(self.iterationSlider, 14, 2, 1, 2)
		self.grid.addWidget(self.iterationReader, 15, 2)

		self.alphaSlider.setValue(5)
		self.gammaSlider.setValue(20)
		self.epsilonSlider.setValue(99)
		self.minEpsilonSlider.setValue(5)
		self.epsilonDecaySlider.setValue(80)
		self.epochsSlider.setValue(100)

		self.frame.setLayout(self.grid)
		self.cw.setLayout(self.grid)

		self.tableHistory = []
		self.policyHistory = []

		self.slice = -1

		self.connectWidgets()
    		
	def connectWidgets(self):
		self.runButton.clicked.connect(self.runBlackjack)
		self.policyButton.clicked.connect(self.showMap)
		self.Qbutton.clicked.connect(self.showMap)
		self.iterationSlider.sliderMoved.connect(self.showMap)

	def runBlackjack(self):
		self.played = True
		self.tableHistory = []
		self.policyHistory = []

		playersDict = {'Casino': CasinoPlayer, 'Random': randomPlayer, 'QLearner': QLearnerPlayer}

		epsilon = self.epsilonSlider.value()/100
		alpha = self.alphaSlider.value()/100
		gamma = self.gammaSlider.value()/100
		minEpsilon = self.minEpsilonSlider.value()/100
		epsilonDecay = self.epsilonDecaySlider.value()/100
		epochs = int(self.epochsSlider.value()/100 * MAXEPOCHS)

		player1 = QLearnerPlayer(gamma, alpha, epsilon, epsilon_decay=epsilonDecay, min_epsilon=minEpsilon)
		player2 = playersDict[self.opponentCombobox.currentText()]()

		seeded_table = np.zeros((22, 13, 2))
		for i in range(10):
			for j in range(13):
				seeded_table[i,j,0] = 2
				seeded_table[i,j,1] = -2
		player1.seed(seeded_table)

		game = Blackjack(player1, player2)

		for i in tqdm(range(epochs)):
			game.play()
			qtable = processQTable(player1.hardQTable)
			policy = player1.getPolicy()
			self.tableHistory.append(deepcopy(qtable))
			self.policyHistory.append(deepcopy(policy))
		maxEpochs = int(self.epochsSlider.value()/100 * MAXEPOCHS)
		position = int(maxEpochs*self.iterationSlider.value()/100)
		fig = plt.imshow(self.tableHistory[position], vmin=-1, vmax=1).figure
		plt.xticks(np.arange(13), [str(i) for i in range(2,11)] + ['J', 'Q', 'K','A'])
		plt.yticks(np.arange(22), [str(i) for i in range(0,22)])
		self.vis = FigureCanvas(fig).draw()

	def showMap(self):
		windowSize = self.size()
		if not self.played:
			print('Play game before using control')
			return
		maxEpochs = int(self.epochsSlider.value()/100 * MAXEPOCHS)
		newSlice = int(self.iterationSlider.value())
		if newSlice == self.slice:
			return
		else:
			self.slice = newSlice
		position = int(maxEpochs*self.slice/100)
		if self.policyButton.isChecked():
			fig = plt.imshow(self.policyHistory[position], vmin=-1, vmax=1).figure
			plt.xticks(np.arange(13), [str(i) for i in range(2,11)] + ['J', 'Q', 'K','A'])
			plt.yticks(np.arange(22), [str(i) for i in range(0,22)])
			self.vis = FigureCanvas(fig)
			self.vis.draw()
		else:
			fig = plt.imshow(self.tableHistory[position], vmin=-1, vmax=1).figure
			plt.xticks(np.arange(13), [str(i) for i in range(2,11)] + ['J', 'Q', 'K','A'])
			plt.yticks(np.arange(22), [str(i) for i in range(0,22)])
			self.vis = FigureCanvas(fig)
			self.vis.draw()
		self.resize(windowSize.width()+2,windowSize.height()+2)
		self.resize(windowSize)

def processQTable(QTable):
	processed = np.zeros((22,13))
	for i in range(22):
		for j in range(13):
			processed[i,j] += (QTable[i,j,0] - QTable[i,j,1])/2
	return processed

'''Launches MainWindow object'''
def launch(filename=None):
	app = QtWidgets.QApplication(sys.argv)
	mw = MainWindow()
	mw.show()
	sys.exit(app.exec_())


'''Pilot'''
launch()

















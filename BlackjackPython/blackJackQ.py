import numpy as np
import random
import time
from copy import deepcopy

class Card:
	def __init__(self, value, suite):
		self.value = value
		self.suite = suite

class Deck:
	def __init__(self):
		self.deck = []
		self.discard = []

	def addCard(self, card):
		self.deck.append(card)

	def draw(self):
		if len(self.deck) == 0:
			self.reshuffle()
		card = self.deck.pop(-1)
		self.discard.append(card)
		return card

	def shuffle(self):
		random.shuffle(self.deck)

	def reshuffle(self):
		self.deck = deepcopy(self.discard)
		self.discard = []
		self.shuffle()

# Interface for players to inherit from
class BlackjackPlayer():
	def __init__(self):
		self.leaner = False

	def play(self, card, showing):
		pass

# Dealer must hit until 17 then stand
class CasinoPlayer(BlackjackPlayer):
	def __init__(self):
		BlackjackPlayer.__init__(self)

	def play(self, cards, showing):
		return currentTotal(cards) < 17

# Plays randomly but holds on 21
class randomPlayer(BlackjackPlayer):
	def __init__(self):
		BlackjackPlayer.__init__(self)

	def play(self, cards, showing):
		# Returns true or false with equal probability
		return bool(random.getrandbits(1)) if currentTotal(cards) < 21 else False

# Is a human
class HumanPlayer(BlackjackPlayer):
	def __init__(self):
		BlackjackPlayer.__init__(self)

	def play(self, cards, showing):
		values = []
		for card in cards:
			values.append(card.value)
		print(values)
		total = currentTotal(cards)
		if total >= 21: return False
		hit = input('Current Total: ', total, '. Hit? (y/n')
		return hit.lower() == 'y'

# Uses Q-Learning algorithm to determine policy
class QLearnerPlayer(BlackjackPlayer):
	def __init__(self, gamma, alpha, epsilon, verbose=False, epsilon_decay=0.95, min_epsilon=0.1):
		BlackjackPlayer.__init__(self)
		self.leaner = True
		self.hardQTable = np.zeros((22, 13, 2))
		self.softQTable = np.zeros((22, 13, 2))
		self.gamma = gamma
		self.alpha = alpha
		self.epsilon = epsilon
		self.epsilon_decay = epsilon_decay
		self.min_epsilon = min_epsilon
		self.verbose = verbose

	def play(self, cards, showing):
		total = currentTotal(cards)
		if total >= 21: return False
		if random.random() >= self.epsilon:
			# Returns true or false with equal probability
			return bool(random.getrandbits(1))
		else:
			return np.argmax(self.hardQTable[total][showing]) == 0

	def learn(self, history):
		# history as a list of triples (s, a, r)
		# state is is a tuple of (value, showing)
		for index, event in enumerate(history):
			total, showing = event[0]
			action = 0 if event[1] == True else 1
			reward = event[2]
			next_state = (-1,-1) if index == len(history) - 1 else history[index+1][0]
			if next_state[0] >= 0:
				next_reward = np.amax(self.hardQTable[next_state])
			else:
				next_reward = 0
			self.hardQTable[total, showing, action] = (1-self.alpha)*self.hardQTable[total, showing, action] + self.alpha*(reward + self.gamma * next_reward)
		# Update epsilon
		self.epsilon = max(self.epsilon_decay*self.epsilon, self.min_epsilon)
		# Print
		if self.verbose:
			print(np.around(self.hardQTable, 2))

	def getPolicy(self):
		choice = np.argmax(self.hardQTable, axis=2)
		return choice

	def seed(self, table):
		self.hardQTable = table

def currentTotal(cards):
		total = 0
		aces = 0
		tens = ['J', 'Q', 'K']
		for card in cards:
			if card.value in tens:
				total += 10
			elif card.value == 'A':
				total += 11
				aces += 1
			else:
				total += int(card.value)
		while total > 21 and aces > 0:
			aces -= 1
			total -= 10
		return total

class Blackjack:
	def __init__(self, player1, player2, decks=1):
		self.player1 = player1
		self.player2 = player2
		# Init game deck
		self.deck = Deck()
		values = ['A', 'J', 'Q', 'K']
		for i in range(9):
			values.append(str(i+2))
		suites = ['hearts', 'clubs', 'diamonds', 'spades']
		for i in range(decks):
			for suite in suites:
				for value in values:
					self.deck.addCard(Card(value, suite))
			self.deck.shuffle()

	def playPlayer(self, player, cards, showing):
		history = []
		showing = CARD2INDEX[showing] - 1
		hit = player.play(cards, showing)
		value = currentTotal(cards)
		history.append([(value, showing), hit, 0])
		while hit and value < 21:
			cards.append(self.deck.draw())
			value = currentTotal(cards)
			hit = player.play(cards, showing)
			if value < 21:
				history.append([(value, showing), hit, 0])
		return history, value

	def play(self):
		p1Cards = [self.deck.draw(), self.deck.draw()]
		p2Cards = [self.deck.draw(), self.deck.draw()]
		p1History, p1Value = self.playPlayer(self.player1, p1Cards, p2Cards[0].value)
		p2History, p2Value = self.playPlayer(self.player2, p2Cards, p1Cards[0].value)
		p1Bust = p1Value > 21
		p2Bust = p2Value > 21
		if p1Bust and not p2Bust:
			p1History[-1][2] = -1
			p2History[-1][2] = 1
		if p2Bust and not p1Bust:
			p1History[-1][2] = 1
			p2History[-1][2] = -1
		if not (p1Bust or p2Bust) and p1Value > p2Value:
			p1History[-1][2] = 1
			p2History[-1][2] = -1
		if not (p1Bust or p2Bust) and p1Value < p2Value:
			p1History[-1][2] = -1
			p2History[-1][2] = 1
		if self.player1.leaner:
			self.player1.learn(p1History)
		if self.player2.leaner:
			self.player2.learn(p2History)

# Index states s.t. 2-> index 0, 3-> index 1, ... J->9, ... A->12
CARD2INDEX = {'J':9, 'Q':10, 'K':11, 'A':12}
for i in range(11):
	CARD2INDEX[str(i + 2)] = i

if __name__ == '__main__':
	gamma = 0.5
	alpha = 0.05
	epsilon = 0.99
	epsilon_decay = 0.95
	min_epsilon = 0.1
	epochs = 10000
	update_interval = 100

	player1 = QLearnerPlayer(gamma, alpha, epsilon, epsilon_decay=epsilon_decay, min_epsilon=min_epsilon)
	player2 = CasinoPlayer()
	game = Blackjack(player1, player2)
	for i in range(epochs):
		if i % update_interval == 0:
			player1.verbose = True
			print('=================')
		game.play()
		player1.verbose = False

	print('=================')
	print('Policy (hit, stand)')
	choice = np.argmax(player1.hardQTable, axis=1)
	policy = np.zeros((22,2))
	for i, el in enumerate(choice):
		policy[i, el] = 1
	print(policy)


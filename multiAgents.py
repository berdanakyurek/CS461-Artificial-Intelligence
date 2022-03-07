# multiAgents.py
# --------------
# Licensing Information:  You are free to use or extend these projects for
# educational purposes provided that (1) you do not distribute or publish
# solutions, (2) you retain this notice, and (3) you provide clear
# attribution to UC Berkeley, including a link to http://ai.berkeley.edu.
#
# Attribution Information: The Pacman AI projects were developed at UC Berkeley.
# The core projects and autograders were primarily created by John DeNero
# (denero@cs.berkeley.edu) and Dan Klein (klein@cs.berkeley.edu).
# Student side autograding was added by Brad Miller, Nick Hay, and
# Pieter Abbeel (pabbeel@cs.berkeley.edu).


from pickletools import floatnl
import sys
from util import manhattanDistance
from game import Directions
import random, util

from game import Agent
from pacman import GameState

class ReflexAgent(Agent):
    """
    A reflex agent chooses an action at each choice point by examining
    its alternatives via a state evaluation function.

    The code below is provided as a guide.  You are welcome to change
    it in any way you see fit, so long as you don't touch our method
    headers.
    """


    def getAction(self, gameState: GameState):
        """
        You do not need to change this method, but you're welcome to.

        getAction chooses among the best options according to the evaluation function.

        Just like in the previous project, getAction takes a GameState and returns
        some Directions.X for some X in the set {NORTH, SOUTH, WEST, EAST, STOP}
        """
        # Collect legal moves and successor states
        legalMoves = gameState.getLegalActions()

        # Choose one of the best actions
        scores = [self.evaluationFunction(gameState, action) for action in legalMoves]
        bestScore = max(scores)
        bestIndices = [index for index in range(len(scores)) if scores[index] == bestScore]
        chosenIndex = random.choice(bestIndices) # Pick randomly among the best

        "Add more of your code here if you want to"

        return legalMoves[chosenIndex]

    def evaluationFunction(self, currentGameState: GameState, action):
        """
        Design a better evaluation function here.

        The evaluation function takes in the current and proposed successor
        GameStates (pacman.py) and returns a number, where higher numbers are better.

        The code below extracts some useful information from the state, like the
        remaining food (newFood) and Pacman position after moving (newPos).
        newScaredTimes holds the number of moves that each ghost will remain
        scared because of Pacman having eaten a power pellet.

        Print out these variables to see what you're getting, then combine them
        to create a masterful evaluation function.
        """
        # Useful information you can extract from a GameState (pacman.py)
        successorGameState = currentGameState.generatePacmanSuccessor(action)
        newPos = successorGameState.getPacmanPosition()
        newFood = successorGameState.getFood()
        newGhostStates = successorGameState.getGhostStates()
        newScaredTimes = [ghostState.scaredTimer for ghostState in newGhostStates]

        "*** YOUR CODE HERE ***"
        import sys
        foodList = newFood.asList() # List of all food available
        closestFood = sys.maxsize   # Arbitrary max distance
        
        # To check for food that is closest
        if foodList:
            closestFood = min(list(map(lambda food: manhattanDistance(newPos, food), foodList)))

        # To check for position of ghost
        ghostPosition = successorGameState.getGhostPositions()    
        # Safest distance while staying as close as possible
        # Normally 2 turns away from the ghost as normal manhattan distance would be 2
        # Considers the case if ghost is less than distance of 2
        closestGhost = True in list(map(lambda ghost: manhattanDistance(newPos, ghost) < 3, ghostPosition))
        if closestGhost:
            return -sys.maxsize
  
        return successorGameState.getScore() + 1/closestFood


def scoreEvaluationFunction(currentGameState: GameState):
    """
    This default evaluation function just returns the score of the state.
    The score is the same one displayed in the Pacman GUI.

    This evaluation function is meant for use with adversarial search agents
    (not reflex agents).
    """
    return currentGameState.getScore()

class MultiAgentSearchAgent(Agent):
    """
    This class provides some common elements to all of your
    multi-agent searchers.  Any methods defined here will be available
    to the MinimaxPacmanAgent, AlphaBetaPacmanAgent & ExpectimaxPacmanAgent.

    You *do not* need to make any changes here, but you can if you want to
    add functionality to all your adversarial search agents.  Please do not
    remove anything, however.

    Note: this is an abstract class: one that should not be instantiated.  It's
    only partially specified, and designed to be extended.  Agent (game.py)
    is another abstract class.
    """

    def __init__(self, evalFn = 'scoreEvaluationFunction', depth = '2'):
        self.index = 0 # Pacman is always agent index 0
        self.evaluationFunction = util.lookup(evalFn, globals())
        self.depth = int(depth)

class MinimaxAgent(MultiAgentSearchAgent):
    """
    Your minimax agent (question 2)
    """

    def getAction(self, gameState: GameState):
        """
        Returns the minimax action from the current gameState using self.depth
        and self.evaluationFunction.

        Here are some method calls that might be useful when implementing minimax.

        gameState.getLegalActions(agentIndex):
        Returns a list of legal actions for an agent
        agentIndex=0 means Pacman, ghosts are >= 1

        gameState.generateSuccessor(agentIndex, action):
        Returns the successor game state after an agent takes an action

        gameState.getNumAgents():
        Returns the total number of agents in the game

        gameState.isWin():
        Returns whether or not the game state is a winning state

        gameState.isLose():
        Returns whether or not the game state is a losing state
        """
        "*** YOUR CODE HERE ***"
        def innerHelper(gs, index, depth):
            if gs.isWin() or gs.isLose() or depth == self.depth:
                return (self.evaluationFunction(gs), None)

            possible_actions = gs.getLegalActions(index)
            utilities = []

            if index+1 == gs.getNumAgents():
                depth += 1
            for act in possible_actions:
                next_step =gs.generateSuccessor(index, act)
                utilities.append(innerHelper(next_step, (index+1)% gs.getNumAgents(), depth)[0])
            result = -1

            if len(utilities) == 0:
                return (self.evaluationFunction(gs), None)
            if index == 0: # Pacman to move
                result = (max(utilities), possible_actions[utilities.index(max(utilities))])
            else:
                result = (min(utilities), possible_actions[utilities.index(min(utilities))])

            #print("result", index, result)
            return result
        outres = innerHelper(gameState, self.index, 0)
        #print(outres)
        return outres[1]
        util.raiseNotDefined()

class AlphaBetaAgent(MultiAgentSearchAgent):

    def getAction(self, gameState: GameState):
        
        def minFunc(gameState, depth, id, alpha, beta):

            posValue = ( sys.maxsize)  #positive biggest num
            if len(gameState.getLegalActions(id)) == 0:
              return (self.evaluationFunction(gameState), 0)
            else:
                for act in gameState.getLegalActions(id):
                    if (id+1 == gameState.getNumAgents()):
                        successor = maxFunc(gameState.generateSuccessor(id, act), depth + 1, alpha, beta)[0]
                    else:
                        successor = minFunc(gameState.generateSuccessor(id, act), depth,id +1, alpha, beta)[0]
                    if (successor < posValue):
                        posValue, action = successor, act

                    if (posValue < alpha):
                        return (posValue, action)

                    beta = min(beta, posValue)

                return (posValue, action)

        def maxFunc(gameState, depth, alpha, beta):

            
            negValuee = -( sys.maxsize) #negative biggest num
            if depth == self.depth or gameState.isWin() or gameState.isLose() or len(gameState.getLegalActions(0)) == 0:
                return (self.evaluationFunction(gameState), 0)
            else :
                for thisAction in gameState.getLegalActions(0):
                    successor = minFunc(gameState.generateSuccessor(0, thisAction), depth, 1, alpha, beta)[0]
                    if (negValuee < successor):
                        negValuee, action = successor, thisAction

                    if (negValuee > beta):
                        return (negValuee, action)

                    alpha = max(alpha, negValuee)

            return (negValuee, action)

        

        return maxFunc(gameState, 0, -( sys.maxsize), ( sys.maxsize))[1]

class ExpectimaxAgent(MultiAgentSearchAgent):
    """
      Your expectimax agent (question 4)
    """
    
    def getAction(self, gameState: GameState):
        """
        Returns the expectimax action using self.depth and self.evaluationFunction

        All ghosts should be modeled as choosing uniformly at random from their
        legal moves.
        """
        "*** YOUR CODE HERE ***"
        def innerHelper(gs, index, depth):
            if gs.isWin() or gs.isLose() or depth == self.depth:
                return (self.evaluationFunction(gs), None)

            possible_actions = gs.getLegalActions(index)
            utilities = []

            if index+1 == gs.getNumAgents():
                depth += 1
            for act in possible_actions:
                next_step =gs.generateSuccessor(index, act)
                utilities.append(innerHelper(next_step, (index+1)% gs.getNumAgents(), depth)[0])
            result = -1

            if len(utilities) == 0:
                return (self.evaluationFunction(gs), None)
            if index == 0: # Pacman to move
                result = (max(utilities), possible_actions[utilities.index(max(utilities))])
            else:
                result = (sum(utilities)/len(utilities), possible_actions[utilities.index(min(utilities))])

            #print("result", index, result)
            return result
        outres = innerHelper(gameState, self.index, 0)
        #print(outres)
        return outres[1]
        util.raiseNotDefined()

def betterEvaluationFunction(currentGameState: GameState):
    """
    Your extreme ghost-hunting, pellet-nabbing, food-gobbling, unstoppable
    evaluation function (question 5).

    DESCRIPTION: 
    <
        Better evaluation function is implemented by considering the following state properties:
            - closest food distance
            - ghost distance (if the pacman is two blocks away from any ghost)
            - number of food left in board
            - number of capsules left in board
        According to those properties, evaluation returns with their responding multipliers that have been initialized before.
    >
    """
    "*** YOUR CODE HERE ***"

    import sys

    pacmanPosition = currentGameState.getPacmanPosition()
    foodList = currentGameState.getFood().asList()
    ghostPositions = currentGameState.getGhostPositions()
    currentScore = currentGameState.getScore()
    closestFood = sys.maxsize

    #find closest food 
    if foodList:
        closestFood = min(list(map(lambda food: manhattanDistance(pacmanPosition, food), foodList)))

    #check ghost position 2 or less
    ghostNearby = True in list(map(lambda ghost: manhattanDistance(pacmanPosition, ghost) < 2, ghostPositions))

    if ghostNearby:
        return -sys.maxsize

    capsulesLeft = len(currentGameState.getCapsules()) + 1
    foodLeft = len(foodList) + 1

    multipliers = [1.0, 3.0, 5.0]
    params = [closestFood, foodLeft, capsulesLeft]

    return currentScore + sum(m / p for m, p in zip(multipliers, params))

# Abbreviation
better = betterEvaluationFunction

'''
CS461 HW1
Group name: SAZAR
Group Members:
Elif Ozdabak
Okan Alp Unver
Nihat Bartu Serttaş
Muharrem Berk Yıldız
Berdan Akyurek
'''


import random
import copy

cannibals = 6
missionaries = 6
boat_size = 5

# Class for a state. Holds the condition of
# cannibals, missionaires and the boat
class state:
    def __init__(self, l, r, bl):
        self.left = l
        self.right = r
        self.boat_location = bl
    
    # This method prints the current state
    def print(self):

        for i in range(self.left[0]):
            print("C", end="")
        for i in range(cannibals-self.left[0]):
            print(" ", end= "")
        print("\t", end="")
        for i in range(self.right[0]):
            print("C", end="")
        print()

        for i in range(self.left[1]):
            print("M", end="")
        for i in range(missionaries-self.left[1]):
            print(" ", end= "")
        print("\t", end="")
        for i in range(self.right[1]):
            print("M", end="")
        print("\n")
    
    # Returns the number of cannibals
    # In the side of the boat
    def get_boatside_cannibals(self):
        if self.boat_location == False:
            return self.left[0]
        else:
            return self.right[0]

    # Returns the number of missionaries
    # In the side of the boat
    def get_boatside_missionaries(self):
        if self.boat_location == False:
            return self.left[1]
        else:
            return self.right[1]

    # Returns the number of cannibals
    # In the opposite side of the boat
    def get_otherside_cannibals(self):
        if self.boat_location == True:
            return self.left[0]
        else:
            return self.right[0]

    # Returns the number of missionaries
    # In the opposite side of the boat
    def get_otherside_missionaries(self):
        if self.boat_location == True:
            return self.left[1]
        else:
            return self.right[1]

    # Returns all possible moves
    def find_moves(self):
        
        possibleMoves = []
        
        # Tries every move and adds the ones that are 
        # possible to a list
        for i in range(cannibals):
            for j in range(missionaries):
                total = i + j
                # Moving with 0 people
                if total == 0 or total > boat_size:
                    continue
                # Not enough cannibals for this move
                if self.get_boatside_cannibals() < i:
                    continue
                # Not enough missionaries for this move
                if self.get_boatside_missionaries() < j:
                    continue
                # Missionaries outnumbered at the boat side
                if self.get_boatside_cannibals()-i > self.get_boatside_missionaries()-j and self.get_boatside_missionaries()-j > 0:
                    continue
                # Missionaries outnumbered at the other side
                if self.get_otherside_cannibals()+i > self.get_otherside_missionaries()+j and self.get_otherside_missionaries()+j > 0:
                    continue
                # add this possible solution to the list
                possibleMoves.append([i,j])

        possibleSts = []
        for move in possibleMoves:
            if self.boat_location == False:
                nL = [self.left[0] - move[0], self.left[1] - move[1]]
                nR = [self.right[0] + move[0], self.right[1] + move[1]]
            else:
                nL = [self.left[0] + move[0], self.left[1] + move[1]]
                nR = [self.right[0] - move[0], self.right[1] - move[1]]
            possibleSts.append(state(nL, nR, not(self.boat_location)))

        return possibleSts

# Queue implementation for nondetermenistic search
class search_queue:

    def __init__(self):
        self.listQ = []
        self.listQ.append([state([cannibals,missionaries], [0,0], False)])

    # returns true if empty
    def is_empty(self):
        if len(self.listQ) == 0:
            return True
        return False

    # adds item to a random place in queue
    # this method makes program nondetermenistic
    def random_add(self, item):

        if self.is_empty():
            self.listQ.append(item)
        else:
            self.listQ.insert(random.randint(0,len(self.listQ)), item)

    # removes and returns first element
    def dequeue(self):
        return self.listQ.pop(0)

    
        
# finds whether arr contains item or not
def contains(arr, item):
    for i in arr:
        if i.left == item.left and i.right == item.right and i.boat_location == item.boat_location:
            return True
    return False

# prints the solution in the expected format
def printSolutionArray(arr):
    print("----------------------------------------")
    for i in range(len(arr)):
        if i != 0:
            strg = ""
            if arr[i].boat_location == True:
                strg += "SEND "
            else:
                strg += "RETURN "
            number = abs(arr[i].left[0] - arr[i-1].left[0])
            strg += str(number)
            strg += " CANNIBALS "
            number = abs(arr[i].left[1] - arr[i-1].left[1])
            strg += str(number)
            strg += " MISSIONARIES"
            print(strg)
        arr[i].print()
    print("----------------------------------------")
    print( len(arr)-1, "RIVER CROSSINGS IN TOTAL")


sq = search_queue()
solution = []

while not sq.is_empty():

    # Remove first element from the 
    # queue and find all possible moves
    current = sq.dequeue()
    if len(current) >= len(solution) and len(solution) != 0:
        continue
    possibleNexts = current[-1].find_moves()

    for i in possibleNexts:
        # If the resulting state already occured before, skip it
        # This way, any loop will be avoided
        if contains(current, i):
            continue
        
        # Create new possible path
        cr = copy.deepcopy(current)
        cr.append(copy.deepcopy(i))

        # If all humans reached to end and new solution
        # is better than current best, set the solution 
        if i.left == [0,0] and (len(cr) < len(solution) or len(solution) == 0):
            solution = cr
            
        sq.random_add(cr)

# Print the final solution
printSolutionArray(solution)
        
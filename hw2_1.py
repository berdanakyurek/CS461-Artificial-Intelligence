import copy
import random
import tkinter
import queue
from dataclasses import dataclass, field
from typing import Any

from anytree import Node, RenderTree


class Puzzle:

    # Initialize sorted puzzle
    def __init__(self, size=4, priority=0):
        self.priority = priority
        self.size = size
        self.puz = []
        init_arr = []
        for i in range(size):
            init_arr.append(i + 1)
        for i in range(size):
            self.puz.append(copy.deepcopy(init_arr))
            init_arr.pop(0)
            init_arr.append(init_arr[-1] - 1)
        self.puz[-1][-1] = 0  # empty cell

    def __eq__(self, other):
        if isinstance(other, Puzzle):
            return self.priority == other.priority
        return False

    def __lt__(self, other):
        if isinstance(other, Puzzle):
            return self.priority > other.priority
        return False

    # return comparison
    def __le__(self, other):
        if isinstance(other, Puzzle):
            return self.priority >= other.priority
        return False

    # return comparison
    def __ne__(self, other):
        if isinstance(other, Puzzle):
            return self.priority != other.priority
        return False

    # return comparison
    def __gt__(self, other):
        if isinstance(other, Puzzle):
            return self.priority < other.priority
        return False

    # return comparison
    def __ge__(self, other):
        if isinstance(other, Puzzle):
            return self.priority <= other.priority
        return False

    # return comparison
    # Prints the current state of puzzle
    def display(self):
        for i in self.puz:
            for j in i:
                if j == 0:
                    print(" ", end=" ")
                else:
                    print(j, end=" ")
            print()
        print()

    # 1 = left
    # 2 = right
    # 3 = up
    # 4 = down
    # this indicates the move
    # direction of empty cell

    # Returns the address of empty cell
    def find_empty_cell(self):
        for i in range(self.size):
            for j in range(self.size):
                if self.puz[i][j] == 0:
                    return [i, j]

    # returns True if move is possible
    # returns False if impossible

    def is_possible(self, move):
        empty_loc = self.find_empty_cell()
        if move == 1:
            if empty_loc[1] > 0:
                return True
        elif move == 2:
            if empty_loc[1] < self.size - 1:
                return True
        elif move == 3:
            if empty_loc[0] > 0:
                return True
        elif move == 4:
            if empty_loc[0] < self.size - 1:
                return True
        return False

    # Returns a list of possible moves
    def get_possible_moves(self):
        ret = []
        for i in range(1, 5):
            if self.is_possible(i):
                ret.append(i)
        return ret

    # Swaps the cell a,b with the cell c,d
    def swap(self, a, b, c, d):
        temp = self.puz[a][b]
        self.puz[a][b] = self.puz[c][d]
        self.puz[c][d] = temp

    # Moves the empty cell once
    # in the specified direction
    # Returns True if success
    # Returns False if not possible
    def move(self, direction):
        if not self.is_possible(direction):
            return False

        empty_loc = self.find_empty_cell()

        if direction == 1:
            self.swap(empty_loc[0], empty_loc[1], empty_loc[0], empty_loc[1] - 1)
        elif direction == 2:
            self.swap(empty_loc[0], empty_loc[1], empty_loc[0], empty_loc[1] + 1)
        elif direction == 3:
            self.swap(empty_loc[0], empty_loc[1], empty_loc[0] - 1, empty_loc[1])
        elif direction == 4:
            self.swap(empty_loc[0], empty_loc[1], empty_loc[0] + 1, empty_loc[1])
        else:
            return False
        return True

    # Performs a single random move
    def random_move(self):
        pos_moves = self.get_possible_moves()
        random_num = random.randint(0, len(pos_moves) - 1)
        self.move(pos_moves[random_num])

    # Shuffles the puzzle with performing
    # random moves given number of time
    def shuffle(self, rep=1000):
        for i in range(rep):
            self.random_move()

    # Solves the puzzle and returns
    # The array of solution as states
    def solve(self, w=2):

        return

    def h(self):
        goal_score = 0
        if self.puz[0][0] == 1:
            goal_score = goal_score + 1
        if self.puz[0][1] == 2:
            goal_score = goal_score + 1
        if self.puz[0][2] == 3:
            goal_score = goal_score + 1
        if self.puz[0][3] == 4:
            goal_score = goal_score + 1
        if self.puz[1][0] == 2:
            goal_score = goal_score + 1
        if self.puz[1][1] == 3:
            goal_score = goal_score + 1
        if self.puz[1][2] == 4:
            goal_score = goal_score + 1
        if self.puz[1][3] == 3:
            goal_score = goal_score + 1
        if self.puz[2][0] == 3:
            goal_score = goal_score + 1
        if self.puz[2][1] == 4:
            goal_score = goal_score + 1
        if self.puz[2][2] == 3:
            goal_score = goal_score + 1
        if self.puz[2][3] == 2:
            goal_score = goal_score + 1
        if self.puz[3][0] == 4:
            goal_score = goal_score + 1
        if self.puz[3][1] == 3:
            goal_score = goal_score + 1
        if self.puz[3][2] == 2:
            goal_score = goal_score + 1
        if self.puz[3][3] == 0:
            goal_score = goal_score + 1
        return goal_score

    def get_leaves(node, max_depth):
        rt = []
        if not node.children:
            rt.append(node)
            return rt

        for child in node.children:
            for leaf in get_leaves(child, max_depth):
                if leaf.depth == max_depth:
                    rt.append(leaf)
        return rt 

    def bfs(self,w = 2 ):
        goal = Puzzle()

        tree = Node(self)
        current = tree
        
        visited = []
        q = queue.PriorityQueue()
        visited.append(self)
        possible_moves = self.get_possible_moves()
        neighbor_list = []
        for i in possible_moves:
            copy_of_current_node = copy.deepcopy(self)
            copy_of_current_node.move(i)
            neighbor_list.append(copy_of_current_node)
        for i in neighbor_list:
            my_bool = False
            for j in visited:
                if i.puz == j.puz:
                    my_bool = True
            if not my_bool:
                i.priority = i.h()
                q.put(i)
        bbool = True
        while bbool:
            llist = []
            for i in range(w):
                if not q.empty():
                    a = q.get()
                    llist.append(a)
                    new = Node(a, parent = current)
            if len(llist) == 0:
                #return tree ################################################
                return self.bfs(w + 1)
            for i in llist:
                if i.puz == goal.puz:
                    new = Node(i, parent = current)
                    print(w)
                    i.display()
                    return new
                    return True
            q = queue.PriorityQueue()
            for i in llist:
                new = Node(i, parent = current)
                current = new
                visited.append(i)
                possible_moves = i.get_possible_moves()
                neighbor_list = []
                new_list = []
                for j in possible_moves:
                    copy_of_current_node = copy.deepcopy(i)
                    copy_of_current_node.move(j)
                    neighbor_list.append(copy_of_current_node)
                    new = Node(copy_of_current_node, parent = current)
                    new_list.append(new)
                aaa = 0
                for j in neighbor_list:
                    my_bool = False
                    for k in visited:
                        if j.puz == k.puz:
                            my_bool = True
                    if not my_bool:
                        j.priority = j.h()
                        q.put(j)
                    aaa += 1



# this function creates a puzzle with given size
# and shuffles it with given number of random moves
def puzzle_generator(rep=1000, size=4):
    a = Puzzle(size)
    a.shuffle(rep)
    return a


# graphically displays the given solution
def solution_display(solution_arr):
    # TODO
    return


#############################
S1 = puzzle_generator()
S2 = puzzle_generator()
S3 = puzzle_generator()

S1.display()
S2.display()
S3.display()

current = S1.bfs()
#sol2 = S2.bfs()
#sol3 = S3.bfs()

for i in range(10):
    print("---------------------------")

x = 0
while x < 10:
    current.name.display()
    current = current.parent

    x += 1
    if current == None:
        break

#solution_display(S1.solve())
#solution_display(S1.solve())
#solution_display(S1.solve())
#############################

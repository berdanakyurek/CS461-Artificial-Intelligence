import copy
import random 
import tkinter
import time

from anytree import Node, RenderTree

class Puzzle:

    # Initialize sorted puzzle 
    def __init__(self, size = 4):
        self.size = size
        self.puz = []
        init_arr = []
        for i in range(size):
            init_arr.append(i + 1)
        for i in range(size):
            self.puz.append(copy.deepcopy(init_arr))
            init_arr.pop(0)
            init_arr.append(init_arr[-1]-1)
        self.puz[-1][-1] = 0 # empty cell

    def __eq__(self, other):
        if(self.puz == other.puz):
            return True
        return False
    
    def __str__(self):
        return ":)"

    # Prints the current state of puzzle
    def display(self):
        for i in self.puz:
            for j in i:
                if j == 0:
                    print(" ", end = " ")
                else:
                    print(j, end = " ")
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
                    return [i,j]

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
        for i in range(1,5):
            if self.is_possible(i):
                ret.append(i)
        return ret

    def get_possible_next_states(self):
        toret = []
        for i in self.get_possible_moves():
            self_cp = copy.deepcopy(self)
            self_cp.move(i)
            toret.append(self_cp)
        return toret
    
    
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
        random_num = random.randint(0,len(pos_moves)-1)
        self.move(pos_moves[random_num])
    
    # Shuffles the puzzle with performing 
    # random moves given number of time 
    def shuffle(self, rep = 100):
        for i in range(rep):
            self.random_move()
    
    def find_all(self, a):

        ret = []

        for i in range(self.size):
            for j in range(self.size):
                if self.puz[i][j] == a:
                    ret.append([i,j])
        return ret
    
    def h(self):
        p_solved = Puzzle(self.size)

        sum = 0
        for i in range(1, self.size+1):
            lst1 = self.find_all(i)
            lst2 = p_solved.find_all(i)
            
            for ii in [value for value in lst1 if value in lst2] :
                lst1.remove(ii)
                lst2.remove(ii)

            for ii in lst1:
                
                for iii in lst2:
                    sum += -1 * manhattan_distance(ii[0], ii[1], iii[0], iii[1])
        e1 = self.find_empty_cell()
        e2 = p_solved.find_empty_cell()
        manh = sum - manhattan_distance(e1[0], e1[1], e2[0], e2[1])
        
        goal_score = 0
        if self.puz[0][0] == 1:
            goal_score = goal_score + 100
        else:
            return manh + goal_score
        if self.puz[0][1] == 2:
            goal_score = goal_score + 90
        else:
            return manh + goal_score
        if self.puz[0][2] == 3 and self.puz[0][3] == 4:
            goal_score = goal_score + 80
        else:
            return manh + goal_score
        if self.puz[1][0] == 2:
            goal_score = goal_score + 70
        else:
            return manh + goal_score
        if self.puz[1][1] == 3:
            goal_score = goal_score + 60
        else:
            return manh + goal_score
        if self.puz[1][2] == 4 and self.puz[1][3] == 3:
            goal_score = goal_score + 50
        else:
            return manh + goal_score

        if self.puz[2][0] == 3 and self.puz[3][0] == 4:
            goal_score = goal_score + 40
        else:
            return manh + goal_score

        if self.puz[2][1] == 4 and self.puz[3][1] == 3:
            goal_score = goal_score + 30
        else:
            return manh + goal_score
        
        if self.puz[2][2] == 3 and self.puz[2][3] == 2 and self.puz[3][2] == 2 and self.puz[3][3] == 0:
            goal_score = goal_score + 9999999999
        
        return goal_score + manh


# this function creates a puzzle with given size
# and shuffles it with given number of random moves
def puzzle_generator(rep = 100, size = 4):
    a = Puzzle(size)
    a.shuffle(rep)
    return a

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

def select_best(leaves, w, size = 4):
    #return leaves
    leavescp = copy.deepcopy(leaves)
    if len(leaves) <= w:
        return leaves
    
    points = []
    sp = Puzzle()
    for i in leavescp:
        point = 0

        for ii in range(size):
            for iii in range(size):
                if i.name.puz[ii][iii] == sp.puz[ii][iii]:
                    point += 1
        points. append(point)
    
    while len(leavescp) > w:
        minin = points.index(min(points))
        points.pop(minin)
        leavescp.pop(minin)
    
    return leavescp 

def manhattan_distance(a, b, c, d):
    return abs(a-c) + abs(b-d)  

def calculate_points(p, size = 4):
    p_solved = Puzzle(size)


    sum = 0
    for i in range(1, size+1):
        lst1 = p.find_all(i)
        lst2 = p_solved.find_all(i)
        
        for ii in [value for value in lst1 if value in lst2] :
            lst1.remove(ii)
            lst2.remove(ii)

        for ii in lst1:
            
            for iii in lst2:
                sum += manhattan_distance(ii[0], ii[1], iii[0], iii[1])
    e1 = p.find_empty_cell()
    e2 = p.find_empty_cell()
    return sum + manhattan_distance(e1[0], e1[1], e2[0], e2[1])



def find_worst(tree, max_depth):
    arr = get_leaves(tree, max_depth)
    points = []
    for i in arr:
        points.append(i.name.h())
    minin = points.index(min(points))
    return arr[minin]

def solve(puzzle, w = 2):
    t0= time.time()
    tree = Node(puzzle)

    solved = Puzzle()
    dep = 0
    minh = 0
    while True:

        for i in get_leaves(tree, dep):
            if i.name.h() > minh:
                print(i.name.h())
                i.name.display()
                minh = i.name.h()
            
            pnss = i.name.get_possible_next_states()

            current = i
            while current.parent != None:
                current = current.parent

                if current.name in pnss:
                    pnss.remove(current.name)

            for ii in pnss:
                new = Node(ii, parent = i)
                if ii.puz == solved.puz:
                    arrSol = []
                    current = new
                    arrSol.append(new.name)
                    while current.parent != None:
                        current = current.parent
                        arrSol.append(current.name)
                    print(time.time() - t0)
                    print("dne")
                    return arrSol
        
        dep += 1

        while len(get_leaves(tree, dep)) > w:
            worst = find_worst(tree, dep)
            
            chldren = list(worst.parent.children)
            chldren.remove(worst)
            worst.parent.children = chldren
        
        if len(get_leaves(tree, dep)) == 0:
            print("rec")
            return solve(puzzle, w+1)
        

def remove_loops(arr):
    for i in range(len(arr)):
        for ii in range(i+1, len(arr)):
            if arr[i] == arr[ii]:
                for iii in range(iii - ii):
                    arr.pop(i)
                remove_loops(arr)
    return arr

def find_path_to_solution(tree):
    # TODO
    return 
# graphically displays the given solution
def solution_display(solution_arr):
    print(len(solution_arr))
    solution_arr.reverse()
    print("Solution")
    print("----------------")
    solution_arr = remove_loops(solution_arr)
    for i in solution_arr:
        i.display()
    print(len(solution_arr))
    return

#############################     
 
S1 = puzzle_generator(40)
#S2 = puzzle_generator()
#S3 = puzzle_generator()

S1.display()
#S2.display()
#S3.display()

solution_display(solve(S1, 2))
print("done1")
#solution_display(solve(S2))
print("done2")
#solution_display(solve(S3))
#############################
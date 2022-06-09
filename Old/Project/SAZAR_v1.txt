# To get data from website
import selenium
from selenium import webdriver

# For headless run
from selenium.webdriver.chrome.options import Options

# For delays 
import time

# To parse the html
from bs4 import BeautifulSoup

# To display
import tkinter

# For date and time
from datetime import datetime

# Options
chrome_options = Options()

while True:    
    x = input("Do you want to run chrome headless (Y/n)? ")
    if x[0] == "y" or x[0] == "Y":
        chrome_options.add_argument("--headless")
        break
    elif x[0] == "N" or x[0] == "n":
        break

# Start the browser

browser = webdriver.Chrome(options=chrome_options)

# Open the NYT puzzle page
browser.get("https://www.nytimes.com/crosswords/game/mini")

# Try until page is loaded
# This while-try-catch block is used for every button click.
# Sometimes click operation fails because page may load slow
# program tries to click a non existing button.
# In this case, catch block is being executed and loop continues.
# When clicking to a button is succesful, loop ends.
i = -1
while True: 
    i += 1
    # There are two types of screens that can appear. 
    # Depending on which, one of the below click operations 
    # will be executed.
    try:
        # Click "OK" button
        browser.find_element_by_xpath("/html/body/div[1]/div/div/div[4]/div/main/div[2]/div/div[2]/div[3]/div/article/div[2]/button").click()
    except:
        try:
            # Click "play without an account" button
            browser.find_element_by_xpath("/html/body/div[1]/div/div/div[4]/div/main/div[2]/div/div[2]/div[3]/div/article/button").click()
        except:
            if i == 5:
                # Sometimes "play without an account" button does not appear.
                # In this case, close and reopen the browser.
                i = -1
                browser.quit()
                browser = webdriver.Chrome()
                browser.get("https://www.nytimes.com/crosswords/game/mini")

            time.sleep(1)
            continue
    break

while True:
    try:
        # Click "Reveal" button
        browser.find_element_by_xpath("/html/body/div[1]/div/div/div[4]/div/main/div[2]/div/div[1]/ul/div[2]/li[2]/button").click()
        
    except:
        time.sleep(1)
        continue
    break

while True: 
    try:
        # Click "Puzzle" button
        browser.find_element_by_xpath("/html/body/div[1]/div/div/div[4]/div/main/div[2]/div/div/ul/div[2]/li[2]/ul/li[3]").click()
    except:
        time.sleep(1)
        continue
    break

while True: 
    try:
        # Click "Reveal" button
        browser.find_element_by_xpath("/html/body/div[1]/div/div[2]/div[2]/article/div[2]/button[2]").click()
    except:
        time.sleep(1)
        continue
    break

while True:
    try: 
        # Click "X" button
        browser.find_element_by_xpath("/html/body/div[1]/div/div[2]/div[2]/span").click()
    except:
        time.sleep(1)
        continue
    break

# Get the page source to parse with BeautifulSoup
html = browser.page_source

# Close the browser since it is not necessary anymore
time.sleep(3)
browser.quit()

# Create the BeautifulSoup object
soup = BeautifulSoup(html, 'html.parser')

# Get the clues
clue_list = soup.find("section", {"class": "Layout-clueLists--10_Xl"}).find_all("div")

# Parse the clues.
# Format: 
# clue_list[0]: across
# clue_list[1]: down
# clue_list[n][k][0]: Clue number.
# clue_list[n][k][1]: Clue text.

for i in range(len(clue_list)):
    clue_list[i] = clue_list[i].find("ol").find_all("li")
    
    for j in range(len(clue_list[i])):
        clue_list[i][j] = clue_list[i][j].find_all("span")

        for k in range(len(clue_list[i][j])):
            clue_list[i][j][k] = clue_list[i][j][k].get_text()

# Print clues for test purposes
'''
print("Across: ")
for i in clue_list[0]:
    print(i)
print()
print("Down: ")
for i in clue_list[1]:
    print(i)
print()
'''

# Get the table html
cells = soup.find("section", {"aria-label": "Game Board"}).find("svg").find("g", {"data-group": "cells"}).find_all("g")

table = []
x = 0

# Parse the table.
# Format: 
# table[n][k]: cell in row n, column k.
# table[n][k][0]: Letter. " " for a black cell
# table[n][k][1]: Corner number. 0 if no corner numbers.
for i in range(5):
    row = []
    for j in range(5):
        cell = ()
        middle = cells[x].find("text", {"text-anchor": "middle"})
        if middle == None:
            cell = ( " ", 0 )
        else:
            middle = middle.get_text().strip()[0]

            corner = cells[x].find("text", {"text-anchor": "start"})
            if corner == None:
                corner = 0
            else:
                corner = int(corner.get_text())
            cell = (middle, corner)
        
        row.append(cell)
        x += 1
    table.append(row)

# Print table for test purposes
'''
print("Table: ")  
for i in table:
    print(i)
'''


reveal = True
while True:
    x = input("Do you want to reveal the answers (Y/n)? ")

    if x[0] == "y" or x[0] == "Y":
        reveal = True
        break
    elif x[0] == "N" or x[0] == "n":
        reveal = False
        break


form = tkinter.Tk()
form.title("SAZAR_v1")

r = 0

lb = tkinter.Label(text = "")
lb.config(font=("Courier", 15))
lb.grid(row=r, sticky=tkinter.W)

r += 1 
lb = tkinter.Label(text = "Across:")
lb.config(font=("Courier", 44))



lb.grid(row=r, sticky=tkinter.W)
r += 1
for i in clue_list[0]:
    lb = tkinter.Label(text = i[0] + " " + i[1]+ "\t")
    lb.config(font=("Courier", 15))
    lb.grid(row=r, sticky=tkinter.W)
    r += 1

r = 1
lb = tkinter.Label(text = "Down:")
lb.config(font=("Courier", 44))
lb.grid(row=r, column = 1, sticky=tkinter.W)
r += 1

for i in clue_list[1]:
    lb = tkinter.Label(text = i[0] + " " + i[1]+ "\t" )
    lb.config(font=("Courier", 15))
    lb.grid(row=r, column = 1, sticky=tkinter.W)
    r += 1

size = 500
edge_dist = 10
canvas = tkinter.Canvas(width=size + 2* edge_dist + 500 , height=size + 2 * edge_dist)

sq_edge = size/5

for i in range(-1,5):
    canvas.create_line(edge_dist + (i+1) * sq_edge, edge_dist, edge_dist + (i+1) * sq_edge, edge_dist + size )
    canvas.create_line( edge_dist, edge_dist + (i+1) * sq_edge, edge_dist + size,  edge_dist + (i+1) * sq_edge )

for i in range(5):
    top_left = (edge_dist, edge_dist + i * (sq_edge))
    for j in range(5):
        if table[i][j][0] == ' ':
            canvas.create_rectangle(top_left[0] , top_left[1], sq_edge+top_left[0], sq_edge+top_left[1], fill='black')
        if table[i][j][1] != 0:
            canvas.create_text(top_left[0] + 10, top_left[1] + 17, text = str(table[i][j][1]), font="Times 20 italic bold")

        if reveal and not table[i][j][0] == ' ': 
            canvas.create_text(top_left[0] + sq_edge/2, top_left[1] + sq_edge/2, text = str(table[i][j][0]), font="Times 40 bold")
        
        top_left = (top_left[0] + sq_edge, top_left[1])


r += 1
signature = datetime.now().strftime("%b %d %Y\n%H:%M:%S\nSAZAR")
lb = tkinter.Label(text = signature )
canvas.create_text(top_left[0] + sq_edge, top_left[1] + sq_edge/2, text = signature, font="Times 15 bold")
canvas.grid(row=r, sticky=tkinter.W, columnspan = 2)

form.mainloop()

import pygame
from dataclasses import dataclass
pygame.init()


class Node:
    def __init__(self, parent, data):
        self.data = data
        self.parent = parent
        self.children = []
        self.level = 0
        self.rect = pygame.Rect(0, 0, NODE_WIDTH, NODE_HEIGHT)


@dataclass
class Line:
    x1: int
    y1: int
    x2: int
    y2: int


WINDOW_WIDTH = 1280
WINDOW_HEIGHT = 800
NODE_WIDTH = 50
NODE_HEIGHT = 50
MARGIN = 10

# recipes = {
#     "Ankh Shield": ["Ankh Charm", "Obsidian Shield"],
#     "Ankh Charm": ["Blindfold", "Armor Bracing", "Medicated Bandage", "Countercurse Mantra", "The Plan"],
#     "Armor Bracing": ["Armor Polish", "Vitamins"],
#     "Medicated Bandage": ["Adhesive Bandage", "Bezoar"],
#     "Countercurse Mantra": ["Nazar", "Megaphone"],
#     "The Plan": ["Trifold Map", "Fast Clock"],
#     "Obsidian Shield": ["Cobalt Shield", "Obsidian Skull"],
#     "Obsidian Skull": ["Obsidian"]
# }
# recipes = {
#     "Ankh Shield":["Obsidian Shield", "Ankh Charm"],
#     "Obsidian Shield":["Cobalt Shield", "Obsidian Skull"],
#     "Obsidian Skull":["Obsidian"],
#     "Ankh Charm": ["Armor Bracing", "Medicated Bandage", "The Plan", "Countercurse Mantra", "Blindfold"],
#     "Armor Bracing":["Armor Polish", "Vitamins"],
#     "Medicated Bandage":["Bezoar", "Adhesive Bandage"],
#     "The Plan":["Fast Clock", "Trifold Map"],
#     "Countercurse Mantra":["Megaphone", "Nazar"]
# }

recipes = {"Terra Blade":["True Night's Edge", "True Excalibur"],"True Night's Edge":["Night's Edge", "Broken Hero Sword"],"Night's Edge":["Blood Butcherer", "Muramasa", "Blade of Grass", "Fiery Greatsword"],"Blood Butcherer":["Crimtane Bar"],"Crimtane Bar":["Crimtane Ore"],"Blade of Grass":["Jungle Spores", "Stinger"],"Fiery Greatsword":["Hellstone Bar"],"Hellstone Bar":["Hellstone", "Obsidian"],"True Excalibur":["Excalibur", "Broken Hero Sword"],"Excalibur":["Hallowed Bar"],}

root_name = "Terra Blade"


def generate_tree(parent: Node, tree_dict: dict):
    if tree_dict.get(parent.data, False):
        for child in tree_dict[parent.data]:
            parent.children.append(generate_tree(Node(parent, child), tree_dict))
    return parent


def draw_tree(n: Node):
    # Pre order
    for c in n.children:
        pygame.draw.rect(display, (0, 255, 0), c.rect)
        display.blit(f.render(c.data, True, (255, 255, 255)), c.rect)
        draw_tree(c)


def set_levels(root, level):
    for child in root.children:
        child.level = level
        set_levels(child, level + 1)


def get_end_nodes(root, end_nodes):
    for child in root.children:
        if len(child.children) == 0:
            end_nodes.append(child)
        get_end_nodes(child, end_nodes)
    return end_nodes


def set_coords(root):
    area_width = WINDOW_WIDTH
    area_height = WINDOW_HEIGHT
    # 1. Find the item on the end of the chain and place according to level
    # 2. Place the parents of the children and draw lines, ONLY IF ALL INGREDIENTS HAVE ALREADY BEEN PLACED
    # 3. Repeat 2

    # List of nodes that have no children
    end_nodes = get_end_nodes(root, [])

    # Calculate node spacing
    num_of_nodes = len(end_nodes)
    min_area_width = ((num_of_nodes - 1) * MARGIN) + (num_of_nodes * NODE_WIDTH)
    area_width = min_area_width# if min_area_width > area_width else area_width
    area_remainder = area_width - (NODE_WIDTH * num_of_nodes)
    inter_node_area = area_remainder / (num_of_nodes - 1)

    # Position end nodes
    for pos, en in enumerate(end_nodes):
        en.rect.x = (NODE_WIDTH * pos) + (inter_node_area * pos)
        en.rect.y = (en.level - 1) * (NODE_HEIGHT * 2)

    # Create list to contain the current end nodes
    cur_nodes = end_nodes.copy()

    h_lines = []

    # Loop until the root is reached
    while cur_nodes:
        rm_list = []  # Remove list
        ap_list = []  # Append list
        for cn in cur_nodes:
            # Parent is None if the node is the root
            if cn.parent is not None:
                # True if the current nodes parent has all of its ingredients on the top level
                if set(cn.parent.children).issubset(cur_nodes) and cn not in rm_list:
                    print(cn.rect.x, cn.rect.y, cn.data)
                    cn.parent.rect.x = cn.parent.children[0].rect.x + ((cn.parent.children[-1].rect.x - cn.parent.children[0].rect.x) / 2)
                    cn.parent.rect.y = cn.rect.y - (NODE_HEIGHT * 2)

                    hl = Line(
                        cn.parent.children[0].rect.centerx,
                        cn.parent.children[0].rect.centery - NODE_HEIGHT,
                        cn.parent.children[-1].rect.centerx,
                        cn.parent.children[-1].rect.centery - NODE_HEIGHT
                    )

                    h_lines.append(hl)

                    # Schedule all of the parents children to be removed from the current list because their positions will have been set
                    for n in cn.parent.children:
                        rm_list.append(n)

                    # Schedule the current nodes parent to be appended because it is now at the top level
                    ap_list.append(cn.parent)

            else:
                # Set the root nodes x coord, no need to set y because the y will always be 0 as it is at the top
                cn.rect.x = cn.children[0].rect.x + ((cn.children[-1].rect.x - cn.children[0].rect.x) / 2)
                rm_list.append(cn)

        # Remove all scheduled nodes
        for rn in rm_list:
            cur_nodes.remove(rn)

        # Append all scheduled nodes
        for an in ap_list:
            cur_nodes.append(an)

    return root, h_lines


def make_lines_diagonal(root, ls):
    for child in root.children:
        ls.append(Line(root.rect.centerx, root.rect.centery, child.rect.centerx, child.rect.centery))
        make_lines_diagonal(child, ls)
    return ls


def make_lines_rectangular(root, ls):
    if len(root.children) > 0:
        ls.append(Line(
            root.rect.centerx,
            root.rect.y + NODE_HEIGHT,
            root.rect.centerx,
            root.rect.centery + NODE_HEIGHT
        ))
    for child in root.children:
        ls.append(Line(
            child.rect.centerx,
            child.rect.y,
            child.rect.centerx,
            child.rect.centery - NODE_HEIGHT
        ))
        make_lines_rectangular(child, ls)
    return ls


def apply_reflection_node(root, max_height):
    root.rect.y = (-root.rect.y) + max_height
    for child in root.children:
        apply_reflection_node(child, max_height)


def apply_reflection_lines(ls, max_height):
    for l in ls:
        l.y1 = (-l.y1) + max_height
        l.y2 = (-l.y2) + max_height


def apply_scale(root, scale):
    pass


def BFS(root):
    seen = [root]
    q = [root]
    max_lvl = 0
    while q:
        v = q.pop(0)
        if v.level > max_lvl:
            max_lvl = v.level
        for e in v.children:
            if e not in seen:
                seen.append(e)
                q.append(e)
    return max_lvl


tree = generate_tree(Node(None, root_name), recipes)
tree.level = 1
set_levels(tree, 2)
root_node, horizontal_lines = set_coords(tree)
lines = make_lines_rectangular(root_node, []) + horizontal_lines
# lines = make_lines_diagonal(root_node, [])

# apply_reflection_node(tree, 300)
# apply_reflection_lines(lines, 300 + NODE_HEIGHT)

display = pygame.display.set_mode((WINDOW_WIDTH, WINDOW_HEIGHT))
clock = pygame.time.Clock()
f = pygame.font.SysFont("Courier", 12, True)

max_level = BFS(root_node)
max_height = (max_level - 1) * (NODE_HEIGHT * 2)

running = True
while running:
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False

        elif event.type == pygame.KEYDOWN:
            if event.key == pygame.K_ESCAPE:
                running = False
            elif event.key == pygame.K_SPACE:
                apply_reflection_node(tree, max_height)
                apply_reflection_lines(lines, max_height + NODE_HEIGHT)

    display.fill((0, 0, 0))

    # Draw lines underneath the rects
    for l in lines:
        print(l.y2 - l.y1)
        pygame.draw.aaline(display, (255, 255, 255), (l.x1, l.y1), (l.x2, l.y2))

    # Draw root node rect and text
    pygame.draw.rect(display, (0, 255, 0), root_node.rect)
    display.blit(f.render(root_node.data, True, (255, 255, 255)), root_node.rect)

    # Draw all children nodes
    print("------------------------------")
    draw_tree(root_node)

    pygame.display.update()
    clock.tick(60)

pygame.quit()

CC      = cc
CFLAGS  = -std=c17 -DNDEBUG -Wall -Wextra -Wpedantic -O2
CFLAGS += -march=native
CFLAGS += -flto
LDFLAGS += -flto

TARGET  = benchmark
BUILDDIR = build
SRCDIR  = src

SRC = $(SRCDIR)/main.c
OBJ = $(SRC:$(SRCDIR)/%.c=$(BUILDDIR)/%.o)

all: $(BUILDDIR) $(BUILDDIR)/$(TARGET)

$(BUILDDIR):
	mkdir -p $(BUILDDIR)

$(BUILDDIR)/$(TARGET): $(OBJ)
	$(CC) $^ -o $@ $(LDFLAGS)

$(BUILDDIR)/%.o: $(SRCDIR)/%.c
	$(CC) $(CFLAGS) -c $< -o $@

clean:
	rm -rf $(BUILDDIR)

.PHONY: all clean

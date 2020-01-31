import os;
import argparse;

class CsvTable:
    def __init__(self, path):
        self.path = path;
        self.raw = [];
        self.rows = [];
        self.columns = [];

        self.__parse_table(path);

    def __parse_table(self, path):
        f = open(path, 'r');
        
        namesline = f.readline();
        names = self.__parse_line(namesline);

        typesline = f.readline();
        types = self.__parse_line(typesline);

        table_width = len(names);
        if len(types) != table_width:
            raise ValueError('CSV table has inconsistent table width.');

        self.raw.append(names);
        self.raw.append(types);

        for name in names:
            self.columns.append(CsvColumn(self, name));

        row_count = 0;
        prev_row = None;
        for line in f:
            values = self.__parse_line(line);
            if len(values) != table_width:
                raise ValueError('CSV table has inconsistent table width.');

            self.raw.append(values);
            if values[0] != '':
                if prev_row != None:
                    prev_row.end_index = row_count;

                prev_row = CsvRow(self, values[0]);
                prev_row.start_index = row_count;

                self.rows.append(prev_row);

            for i in range(table_width):
                self.columns[i].data.append(values[i]);
            
            row_count += 1;

        if prev_row != None:
            prev_row.end_index = row_count;

        f.close();

    def __parse_line(self, line):
        columns = []
        token = '';
        incommas = False;
        for c in line:
            if c == '"':
                if incommas == True:
                    incommas = False;
                else:
                    incommas = True;
            elif c == ',' and not incommas:
                columns.append(token);
                token = '';
            else:
                token += c;
        
        return columns;

class CsvRow:
    def __init__(self, table, name):
        self.__table = table;
        self.name = name;
        self.start_index = 0;
        self.end_index = 0;

class CsvColumn:
    def __init__(self, table, name):
        self.__table = table;
        self.name = name;
        self.data = [];

class CodeGen:
    def generate(self, table):
        print(' generating code for table at \'{0}\''.format(table.path));
        names = table.raw[0];
        types = table.raw[1];
        if names == None or types == None:
            raise ValueError('CSV table did not contain rows defining names and types of column.');

        output = ''
        for i in range(len(names)):
            typestr = self.__format_type(types[i]);
            namestr = self.__format_name(names[i]);
            postfix = self.__get_postfix(table, i);
            output += 'public {0}{1} {2} {{ get; set; }}\n'.format(typestr, postfix, namestr);

        return output;

    def __format_type(self, typestr):
        ret = typestr.lower();
        if ret == 'boolean':
            ret = 'bool';
        return ret;

    def __format_name(self, namestr):
        return namestr;

    def __get_postfix(self, table, column_index):
        column = table.columns[column_index];
        for j in range(len(table.rows)):
            row = table.rows[j];
            lvls = row.end_index - row.start_index;
            for i in range(lvls):
                d = column.data[row.start_index + i];
                if d != '' and i != 0:
                    return '[]';
        return '';

    def __get_summary(self, column):
        d = '';
        first = True;
        was_upper = False;
        for c in column:
            if c.isupper():
                if first or c == '_':
                    d += c;
                else:
                    if was_upper:
                        d += c;
                    else:
                        d += ' ' + c.lower();
                was_upper = True;
            else:
                d += c;
                was_upper = False;
            first = False;

        summary = '/// <summary>\n/// {0}\n/// </summary>\n';
        comment = 'Gets or sets the {0}'.format(d);
               
        return summary.format(comment);
def main():
    parser = argparse.ArgumentParser(description='produces C# properties from the specified csv files');
    parser.add_argument('files', help='csv files from which to produce the properties', nargs='+'); 
    parser.add_argument('-o', '--output', help='output directory the produced properties', default='out');
    args = parser.parse_args();
   
    if not os.path.exists(args.output):
        os.makedirs(args.output);
   
    print(args.files);
    gen = CodeGen();
    for csv in args.files:
       table = CsvTable(csv);
       code = gen.generate(table); 
       file_name = os.path.basename(csv);
       file_name = os.path.splitext(file_name)[0];

       outpath = os.path.join(args.output, file_name + '_properties.cs');

       f = open(outpath, 'w');
       f.write(code);
       f.close();

if __name__ == "__main__":
    main();
